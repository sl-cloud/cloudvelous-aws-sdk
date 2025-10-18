using Cloudvelous.Aws.Sqs;
using FluentAssertions;

namespace Cloudvelous.Aws.Sqs.Tests;

public class SqsMessageTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var message = new SqsMessage<string>();

        // Assert
        message.MessageId.Should().BeEmpty();
        message.ReceiptHandle.Should().BeEmpty();
        message.Body.Should().BeNull();
        message.Attributes.Should().NotBeNull();
        message.Attributes.Should().BeEmpty();
        message.SystemAttributes.Should().NotBeNull();
        message.SystemAttributes.Should().BeEmpty();
        message.ApproximateReceiveCount.Should().Be(0);
        message.ApproximateFirstReceiveTimestamp.Should().BeNull();
        message.SentTimestamp.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var message = new SqsMessage<string>();
        var messageId = "test-message-id";
        var receiptHandle = "test-receipt-handle";
        var body = "test-message-body";
        var attributes = new Dictionary<string, string> { { "key", "value" } };
        var systemAttributes = new Dictionary<string, string> { { "system-key", "system-value" } };
        var receiveCount = 5;
        var firstReceiveTime = DateTime.UtcNow;
        var sentTime = DateTime.UtcNow.AddMinutes(-1);

        // Act
        message.MessageId = messageId;
        message.ReceiptHandle = receiptHandle;
        message.Body = body;
        message.Attributes = attributes;
        message.SystemAttributes = systemAttributes;
        message.ApproximateReceiveCount = receiveCount;
        message.ApproximateFirstReceiveTimestamp = firstReceiveTime;
        message.SentTimestamp = sentTime;

        // Assert
        message.MessageId.Should().Be(messageId);
        message.ReceiptHandle.Should().Be(receiptHandle);
        message.Body.Should().Be(body);
        message.Attributes.Should().BeEquivalentTo(attributes);
        message.SystemAttributes.Should().BeEquivalentTo(systemAttributes);
        message.ApproximateReceiveCount.Should().Be(receiveCount);
        message.ApproximateFirstReceiveTimestamp.Should().Be(firstReceiveTime);
        message.SentTimestamp.Should().Be(sentTime);
    }

    [Fact]
    public void SqsMessage_ShouldBeGeneric()
    {
        // Act
        var stringMessage = new SqsMessage<string>();
        var intMessage = new SqsMessage<int>();
        var objectMessage = new SqsMessage<object>();

        // Assert
        stringMessage.Should().NotBeNull();
        intMessage.Should().NotBeNull();
        objectMessage.Should().NotBeNull();
    }
}
