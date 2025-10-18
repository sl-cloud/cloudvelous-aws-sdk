using Amazon.SQS;
using Amazon.SQS.Model;
using Cloudvelous.Aws.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Cloudvelous.Aws.Sqs;

    /// <summary>
    /// SQS client implementation
    /// </summary>
    public class SqsClient : AwsClientFactoryBase<AmazonSQSClient, SqsOptions>, ISqsClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        private readonly IAmazonSQS _sqsClient;

        /// <summary>
        /// Initializes a new instance of the SQS client
        /// </summary>
        /// <param name="options">SQS configuration options</param>
        /// <param name="logger">Logger instance</param>
        public SqsClient(IOptions<SqsOptions> options, ILogger<SqsClient> logger) : base(options, logger)
        {
            _sqsClient = CreateClient();
        }

        /// <summary>
        /// Initializes a new instance of the SQS client with a specific client instance (for testing)
        /// </summary>
        /// <param name="options">SQS configuration options</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="sqsClient">SQS client instance</param>
        internal SqsClient(IOptions<SqsOptions> options, ILogger<SqsClient> logger, IAmazonSQS sqsClient) : base(options, logger)
        {
            _sqsClient = sqsClient;
        }

    /// <inheritdoc />
    protected override Amazon.Runtime.ClientConfig CreateSpecificClientConfig()
    {
        return new AmazonSQSConfig();
    }

    /// <inheritdoc />
    protected override AmazonSQSClient CreateClientInstance(Amazon.Runtime.ClientConfig config)
    {
        return new AmazonSQSClient((AmazonSQSConfig)config);
    }

    /// <inheritdoc />
    public async Task<string> SendMessageAsync<T>(string queueUrl, T messageBody, Dictionary<string, string>? messageAttributes = null, int delaySeconds = 0, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Sending message to SQS queue: {QueueUrl}", queueUrl);

        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(messageBody, JsonOptions),
            DelaySeconds = delaySeconds,
            MessageAttributes = new Dictionary<string, MessageAttributeValue>()
        };

        if (messageAttributes != null)
        {
            foreach (var attribute in messageAttributes)
            {
                request.MessageAttributes.Add(attribute.Key, new MessageAttributeValue 
                { 
                    StringValue = attribute.Value, 
                    DataType = "String" 
                });
            }
        }

        var response = await _sqsClient.SendMessageAsync(request, cancellationToken);
        return response.MessageId;
    }

    /// <inheritdoc />
    public async Task<List<BatchResultErrorEntry>> SendMessageBatchAsync<T>(string queueUrl, IEnumerable<T> messages, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Sending {MessageCount} messages to SQS queue: {QueueUrl}", messages.Count(), queueUrl);

        var entries = messages.Select((message, index) => new SendMessageBatchRequestEntry
        {
            Id = Guid.NewGuid().ToString(),
                MessageBody = JsonSerializer.Serialize(message, JsonOptions)
        }).ToList();

        var request = new SendMessageBatchRequest
        {
            QueueUrl = queueUrl,
            Entries = entries
        };

        var response = await _sqsClient.SendMessageBatchAsync(request, cancellationToken);
        return response.Failed;
    }

    /// <inheritdoc />
    public async Task<List<SqsMessage<T>>> ReceiveMessagesAsync<T>(string queueUrl, int? maxMessages = null, int? waitTimeSeconds = null, int? visibilityTimeoutSeconds = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Receiving messages from SQS queue: {QueueUrl}", queueUrl);

        var request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = maxMessages ?? Options.MaxReceiveMessages,
            WaitTimeSeconds = waitTimeSeconds ?? Options.DefaultReceiveMessageWaitTimeSeconds,
            VisibilityTimeout = visibilityTimeoutSeconds ?? Options.DefaultVisibilityTimeoutSeconds,
            MessageAttributeNames = new List<string> { "All" },
            MessageSystemAttributeNames = new List<string> { "All" }
        };

        var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);
        return response.Messages.Select(SqsMessage<T>.FromAmazonSqsMessage).ToList();
    }

    /// <inheritdoc />
    public async Task DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Deleting message from SQS queue: {QueueUrl} with ReceiptHandle: {ReceiptHandle}", queueUrl, receiptHandle);

        var request = new DeleteMessageRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = receiptHandle
        };

        await _sqsClient.DeleteMessageAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<BatchResultErrorEntry>> DeleteMessageBatchAsync(string queueUrl, IEnumerable<string> receiptHandles, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Deleting {MessageCount} messages from SQS queue: {QueueUrl}", receiptHandles.Count(), queueUrl);

        var entries = receiptHandles.Select((handle, index) => new DeleteMessageBatchRequestEntry
        {
            Id = Guid.NewGuid().ToString(),
            ReceiptHandle = handle
        }).ToList();

        var request = new DeleteMessageBatchRequest
        {
            QueueUrl = queueUrl,
            Entries = entries
        };

        var response = await _sqsClient.DeleteMessageBatchAsync(request, cancellationToken);
        return response.Failed;
    }

    /// <inheritdoc />
    public async Task ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle, int visibilityTimeoutSeconds, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Changing message visibility for SQS queue: {QueueUrl} with ReceiptHandle: {ReceiptHandle} to {VisibilityTimeoutSeconds} seconds", queueUrl, receiptHandle, visibilityTimeoutSeconds);

        var request = new ChangeMessageVisibilityRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = receiptHandle,
            VisibilityTimeout = visibilityTimeoutSeconds
        };

        await _sqsClient.ChangeMessageVisibilityAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting SQS queue URL for queue: {QueueName}", queueName);

        var request = new GetQueueUrlRequest
        {
            QueueName = queueName
        };

        var response = await _sqsClient.GetQueueUrlAsync(request, cancellationToken);
        return response.QueueUrl;
    }

    /// <inheritdoc />
    public async Task<string> CreateQueueAsync(string queueName, Dictionary<string, string>? attributes = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Creating SQS queue: {QueueName}", queueName);

        var request = new CreateQueueRequest
        {
            QueueName = queueName,
            Attributes = attributes ?? new Dictionary<string, string>
            {
                { "VisibilityTimeout", Options.DefaultVisibilityTimeoutSeconds.ToString() },
                { "MessageRetentionPeriod", Options.DefaultMessageRetentionPeriodSeconds.ToString() }
            }
        };

        var response = await _sqsClient.CreateQueueAsync(request, cancellationToken);
        return response.QueueUrl;
    }

    /// <inheritdoc />
    public async Task DeleteQueueAsync(string queueUrl, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Deleting SQS queue: {QueueUrl}", queueUrl);

        var request = new DeleteQueueRequest
        {
            QueueUrl = queueUrl
        };

        await _sqsClient.DeleteQueueAsync(request, cancellationToken);
    }
}