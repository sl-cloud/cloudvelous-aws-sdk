using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Sqs;
using FluentAssertions;

namespace Cloudvelous.Aws.Sqs.Tests;

public class SqsOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new SqsOptions();

        // Assert
        options.Region.Should().Be("us-east-1");
        options.DefaultVisibilityTimeoutSeconds.Should().Be(30);
        options.DefaultMessageRetentionPeriodSeconds.Should().Be(1209600); // 14 days
        options.DefaultReceiveMessageWaitTimeSeconds.Should().Be(20);
        options.MaxReceiveMessages.Should().Be(10);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(300)]
    public void DefaultVisibilityTimeoutSeconds_ShouldAcceptValidValues(int timeout)
    {
        // Arrange
        var options = new SqsOptions();

        // Act
        options.DefaultVisibilityTimeoutSeconds = timeout;

        // Assert
        options.DefaultVisibilityTimeoutSeconds.Should().Be(timeout);
    }

    [Theory]
    [InlineData(60)]
    [InlineData(86400)]
    [InlineData(1209600)]
    public void DefaultMessageRetentionPeriodSeconds_ShouldAcceptValidValues(int retentionPeriod)
    {
        // Arrange
        var options = new SqsOptions();

        // Act
        options.DefaultMessageRetentionPeriodSeconds = retentionPeriod;

        // Assert
        options.DefaultMessageRetentionPeriodSeconds.Should().Be(retentionPeriod);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(20)]
    [InlineData(60)]
    public void DefaultReceiveMessageWaitTimeSeconds_ShouldAcceptValidValues(int waitTime)
    {
        // Arrange
        var options = new SqsOptions();

        // Act
        options.DefaultReceiveMessageWaitTimeSeconds = waitTime;

        // Assert
        options.DefaultReceiveMessageWaitTimeSeconds.Should().Be(waitTime);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void MaxReceiveMessages_ShouldAcceptValidValues(int maxMessages)
    {
        // Arrange
        var options = new SqsOptions();

        // Act
        options.MaxReceiveMessages = maxMessages;

        // Assert
        options.MaxReceiveMessages.Should().Be(maxMessages);
    }

    [Fact]
    public void SqsOptions_ShouldInheritFromAwsClientOptions()
    {
        // Act
        var options = new SqsOptions();

        // Assert
        options.Should().BeAssignableTo<AwsClientOptions>();
    }
}
