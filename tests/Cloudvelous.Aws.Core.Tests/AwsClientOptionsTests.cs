using Cloudvelous.Aws.Core;
using FluentAssertions;

namespace Cloudvelous.Aws.Core.Tests;

public class AwsClientOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new AwsClientOptions();

        // Assert
        options.Region.Should().Be("us-east-1");
        options.AccessKeyId.Should().BeNull();
        options.SecretAccessKey.Should().BeNull();
        options.SessionToken.Should().BeNull();
        options.RequestTimeoutSeconds.Should().Be(30);
        options.RetryPolicy.Should().NotBeNull();
        options.CircuitBreaker.Should().NotBeNull();
    }

    [Fact]
    public void RetryPolicy_ShouldBeInitializedWithDefaultValues()
    {
        // Act
        var options = new AwsClientOptions();

        // Assert
        options.RetryPolicy.MaxRetryAttempts.Should().Be(3);
        options.RetryPolicy.BaseDelayMs.Should().Be(1000);
        options.RetryPolicy.MaxDelayMs.Should().Be(10000);
        options.RetryPolicy.UseExponentialBackoff.Should().BeTrue();
        options.RetryPolicy.JitterFactor.Should().Be(0.1);
    }

    [Fact]
    public void CircuitBreaker_ShouldBeInitializedWithDefaultValues()
    {
        // Act
        var options = new AwsClientOptions();

        // Assert
        options.CircuitBreaker.FailureThreshold.Should().Be(5);
        options.CircuitBreaker.DurationOfBreakSeconds.Should().Be(30);
        options.CircuitBreaker.SamplingDurationSeconds.Should().Be(10);
    }

    [Theory]
    [InlineData("us-west-2")]
    [InlineData("eu-west-1")]
    [InlineData("ap-southeast-1")]
    public void Region_ShouldAcceptValidRegions(string region)
    {
        // Arrange
        var options = new AwsClientOptions();

        // Act
        options.Region = region;

        // Assert
        options.Region.Should().Be(region);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(300)]
    public void RequestTimeoutSeconds_ShouldAcceptValidValues(int timeout)
    {
        // Arrange
        var options = new AwsClientOptions();

        // Act
        options.RequestTimeoutSeconds = timeout;

        // Assert
        options.RequestTimeoutSeconds.Should().Be(timeout);
    }

    [Fact]
    public void Credentials_ShouldBeSettable()
    {
        // Arrange
        var options = new AwsClientOptions();
        var accessKey = "AKIAIOSFODNN7EXAMPLE";
        var secretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
        var sessionToken = "session-token-example";

        // Act
        options.AccessKeyId = accessKey;
        options.SecretAccessKey = secretKey;
        options.SessionToken = sessionToken;

        // Assert
        options.AccessKeyId.Should().Be(accessKey);
        options.SecretAccessKey.Should().Be(secretKey);
        options.SessionToken.Should().Be(sessionToken);
    }
}
