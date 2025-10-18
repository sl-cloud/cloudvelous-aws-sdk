using Amazon.SQS;
using Amazon.SQS.Model;
using Cloudvelous.Aws.Sqs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;

namespace Cloudvelous.Aws.Sqs.Tests;

/// <summary>
/// Unit tests for SqsClient
/// </summary>
public class SqsClientTests
{
    private readonly Mock<IAmazonSQS> _mockSqsClient;
    private readonly Mock<IOptions<SqsOptions>> _mockOptions;
    private readonly Mock<ILogger<SqsClient>> _mockLogger;
    private readonly SqsClient _sqsClient;

    public SqsClientTests()
    {
        _mockSqsClient = new Mock<IAmazonSQS>();
        _mockOptions = new Mock<IOptions<SqsOptions>>();
        _mockLogger = new Mock<ILogger<SqsClient>>();

        _mockOptions.Setup(x => x.Value).Returns(new SqsOptions
        {
            Region = "us-east-1",
            DefaultVisibilityTimeoutSeconds = 30,
            DefaultReceiveMessageWaitTimeSeconds = 20,
            MaxReceiveMessages = 10
        });

        _sqsClient = new SqsClient(_mockOptions.Object, _mockLogger.Object, _mockSqsClient.Object);
    }

    [Fact]
    public async Task SendMessageAsync_WithValidMessage_ShouldReturnMessageId()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var messageBody = new { Text = "Hello World", Id = 123 };
        var expectedMessageId = "test-message-id";

        _mockSqsClient.Setup(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse { MessageId = expectedMessageId });

        // Act
        var result = await _sqsClient.SendMessageAsync(queueUrl, messageBody);

        // Assert
        result.Should().Be(expectedMessageId);
        _mockSqsClient.Verify(x => x.SendMessageAsync(
            It.Is<SendMessageRequest>(req => 
                req.QueueUrl == queueUrl && 
                req.MessageBody.Contains("Hello World")), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_WithMessageAttributes_ShouldIncludeAttributes()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var messageBody = "test message";
        var messageAttributes = new Dictionary<string, string>
        {
            { "Attribute1", "Value1" },
            { "Attribute2", "Value2" }
        };

        _mockSqsClient.Setup(x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse { MessageId = "test-id" });

        // Act
        await _sqsClient.SendMessageAsync(queueUrl, messageBody, messageAttributes);

        // Assert
        _mockSqsClient.Verify(x => x.SendMessageAsync(
            It.Is<SendMessageRequest>(req => 
                req.MessageAttributes.ContainsKey("Attribute1") &&
                req.MessageAttributes.ContainsKey("Attribute2")), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageBatchAsync_WithMultipleMessages_ShouldReturnFailedEntries()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var messages = new[] { "message1", "message2", "message3" };
        var failedEntries = new List<BatchResultErrorEntry>
        {
            new BatchResultErrorEntry { Id = "1", Code = "Error", Message = "Failed" }
        };

        _mockSqsClient.Setup(x => x.SendMessageBatchAsync(It.IsAny<SendMessageBatchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageBatchResponse { Failed = failedEntries });

        // Act
        var result = await _sqsClient.SendMessageBatchAsync(queueUrl, messages);

        // Assert
        result.Should().HaveCount(1);
        result.First().Code.Should().Be("Error");
        _mockSqsClient.Verify(x => x.SendMessageBatchAsync(
            It.Is<SendMessageBatchRequest>(req => req.Entries.Count == 3), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReceiveMessagesAsync_WithValidQueue_ShouldReturnMessages()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var amazonMessages = new List<Message>
        {
            new Message
            {
                MessageId = "msg1",
                ReceiptHandle = "receipt1",
                Body = JsonSerializer.Serialize(new { Text = "Hello" }),
                Attributes = new Dictionary<string, string>
                {
                    { "ApproximateReceiveCount", "1" },
                    { "SentTimestamp", "1640995200000" }
                }
            }
        };

        _mockSqsClient.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = amazonMessages });

        // Act
        var result = await _sqsClient.ReceiveMessagesAsync<string>(queueUrl);

        // Assert
        result.Should().HaveCount(1);
        result.First().MessageId.Should().Be("msg1");
        result.First().ReceiptHandle.Should().Be("receipt1");
        result.First().ApproximateReceiveCount.Should().Be(1);
    }

    [Fact]
    public async Task DeleteMessageAsync_WithValidReceiptHandle_ShouldCallDeleteMessage()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var receiptHandle = "test-receipt-handle";

        _mockSqsClient.Setup(x => x.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteMessageResponse());

        // Act
        await _sqsClient.DeleteMessageAsync(queueUrl, receiptHandle);

        // Assert
        _mockSqsClient.Verify(x => x.DeleteMessageAsync(
            It.Is<DeleteMessageRequest>(req => 
                req.QueueUrl == queueUrl && 
                req.ReceiptHandle == receiptHandle), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMessageBatchAsync_WithMultipleReceiptHandles_ShouldReturnFailedEntries()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var receiptHandles = new[] { "receipt1", "receipt2", "receipt3" };
        var failedEntries = new List<BatchResultErrorEntry>
        {
            new BatchResultErrorEntry { Id = "1", Code = "Error", Message = "Failed" }
        };

        _mockSqsClient.Setup(x => x.DeleteMessageBatchAsync(It.IsAny<DeleteMessageBatchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteMessageBatchResponse { Failed = failedEntries });

        // Act
        var result = await _sqsClient.DeleteMessageBatchAsync(queueUrl, receiptHandles);

        // Assert
        result.Should().HaveCount(1);
        result.First().Code.Should().Be("Error");
        _mockSqsClient.Verify(x => x.DeleteMessageBatchAsync(
            It.Is<DeleteMessageBatchRequest>(req => req.Entries.Count == 3), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangeMessageVisibilityAsync_WithValidParameters_ShouldCallChangeVisibility()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var receiptHandle = "test-receipt-handle";
        var visibilityTimeout = 60;

        _mockSqsClient.Setup(x => x.ChangeMessageVisibilityAsync(It.IsAny<ChangeMessageVisibilityRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeMessageVisibilityResponse());

        // Act
        await _sqsClient.ChangeMessageVisibilityAsync(queueUrl, receiptHandle, visibilityTimeout);

        // Assert
        _mockSqsClient.Verify(x => x.ChangeMessageVisibilityAsync(
            It.Is<ChangeMessageVisibilityRequest>(req => 
                req.QueueUrl == queueUrl && 
                req.ReceiptHandle == receiptHandle &&
                req.VisibilityTimeout == visibilityTimeout), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetQueueUrlAsync_WithValidQueueName_ShouldReturnQueueUrl()
    {
        // Arrange
        var queueName = "test-queue";
        var expectedUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";

        _mockSqsClient.Setup(x => x.GetQueueUrlAsync(It.IsAny<GetQueueUrlRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueueUrlResponse { QueueUrl = expectedUrl });

        // Act
        var result = await _sqsClient.GetQueueUrlAsync(queueName);

        // Assert
        result.Should().Be(expectedUrl);
        _mockSqsClient.Verify(x => x.GetQueueUrlAsync(
            It.Is<GetQueueUrlRequest>(req => req.QueueName == queueName), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateQueueAsync_WithValidQueueName_ShouldReturnQueueUrl()
    {
        // Arrange
        var queueName = "test-queue";
        var expectedUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";
        var attributes = new Dictionary<string, string>
        {
            { "VisibilityTimeout", "30" },
            { "MessageRetentionPeriod", "1209600" }
        };

        _mockSqsClient.Setup(x => x.CreateQueueAsync(It.IsAny<CreateQueueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateQueueResponse { QueueUrl = expectedUrl });

        // Act
        var result = await _sqsClient.CreateQueueAsync(queueName, attributes);

        // Assert
        result.Should().Be(expectedUrl);
        _mockSqsClient.Verify(x => x.CreateQueueAsync(
            It.Is<CreateQueueRequest>(req => 
                req.QueueName == queueName &&
                req.Attributes.ContainsKey("VisibilityTimeout")), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteQueueAsync_WithValidQueueUrl_ShouldCallDeleteQueue()
    {
        // Arrange
        var queueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue";

        _mockSqsClient.Setup(x => x.DeleteQueueAsync(It.IsAny<DeleteQueueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteQueueResponse());

        // Act
        await _sqsClient.DeleteQueueAsync(queueUrl);

        // Assert
        _mockSqsClient.Verify(x => x.DeleteQueueAsync(
            It.Is<DeleteQueueRequest>(req => req.QueueUrl == queueUrl), 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
