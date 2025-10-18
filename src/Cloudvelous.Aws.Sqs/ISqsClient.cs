using Amazon.SQS.Model;

namespace Cloudvelous.Aws.Sqs;

/// <summary>
/// SQS client interface
/// </summary>
public interface ISqsClient
{
    /// <summary>
    /// Sends a message to the specified queue
    /// </summary>
    /// <typeparam name="T">Message body type</typeparam>
    /// <param name="queueUrl">Queue URL</param>
    /// <param name="messageBody">Message body</param>
    /// <param name="messageAttributes">Optional message attributes</param>
    /// <param name="delaySeconds">Delay in seconds before message becomes available</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Message ID</returns>
    Task<string> SendMessageAsync<T>(string queueUrl, T messageBody, Dictionary<string, string>? messageAttributes = null, int delaySeconds = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends multiple messages to the specified queue
    /// </summary>
    /// <typeparam name="T">Message body type</typeparam>
    /// <param name="queueUrl">Queue URL</param>
    /// <param name="messages">Messages to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Results of the batch operation</returns>
    Task<List<BatchResultErrorEntry>> SendMessageBatchAsync<T>(string queueUrl, IEnumerable<T> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Receives messages from the specified queue
    /// </summary>
    /// <typeparam name="T">Message body type</typeparam>
    /// <param name="queueUrl">Queue URL</param>
    /// <param name="maxMessages">Maximum number of messages to receive</param>
    /// <param name="waitTimeSeconds">Long polling wait time</param>
    /// <param name="visibilityTimeoutSeconds">Visibility timeout for received messages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Received messages</returns>
    Task<List<SqsMessage<T>>> ReceiveMessagesAsync<T>(string queueUrl, int? maxMessages = null, int? waitTimeSeconds = null, int? visibilityTimeoutSeconds = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a message from the queue
    /// </summary>
    /// <param name="queueUrl">Queue URL</param>
    /// <param name="receiptHandle">Receipt handle from received message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteMessageAsync(string queueUrl, string receiptHandle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple messages from the queue
    /// </summary>
    /// <param name="queueUrl">Queue URL</param>
    /// <param name="receiptHandles">Receipt handles from received messages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Results of the batch operation</returns>
    Task<List<BatchResultErrorEntry>> DeleteMessageBatchAsync(string queueUrl, IEnumerable<string> receiptHandles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the visibility timeout of a message
    /// </summary>
    /// <param name="queueUrl">Queue URL</param>
    /// <param name="receiptHandle">Receipt handle from received message</param>
    /// <param name="visibilityTimeoutSeconds">New visibility timeout</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ChangeMessageVisibilityAsync(string queueUrl, string receiptHandle, int visibilityTimeoutSeconds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the URL of a queue by name
    /// </summary>
    /// <param name="queueName">Queue name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Queue URL</returns>
    Task<string> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a queue if it doesn't exist
    /// </summary>
    /// <param name="queueName">Queue name</param>
    /// <param name="attributes">Queue attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Queue URL</returns>
    Task<string> CreateQueueAsync(string queueName, Dictionary<string, string>? attributes = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a queue
    /// </summary>
    /// <param name="queueUrl">Queue URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteQueueAsync(string queueUrl, CancellationToken cancellationToken = default);
}
