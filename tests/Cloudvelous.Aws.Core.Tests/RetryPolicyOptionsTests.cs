using Cloudvelous.Aws.Core;
using FluentAssertions;

namespace Cloudvelous.Aws.Core.Tests;

public class RetryPolicyOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new RetryPolicyOptions();

        // Assert
        options.MaxRetryAttempts.Should().Be(3);
        options.BaseDelayMs.Should().Be(1000);
        options.MaxDelayMs.Should().Be(10000);
        options.UseExponentialBackoff.Should().BeTrue();
        options.JitterFactor.Should().Be(0.1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void MaxRetryAttempts_ShouldAcceptValidValues(int maxRetries)
    {
        // Arrange
        var options = new RetryPolicyOptions();

        // Act
        options.MaxRetryAttempts = maxRetries;

        // Assert
        options.MaxRetryAttempts.Should().Be(maxRetries);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(5000)]
    public void BaseDelayMs_ShouldAcceptValidValues(int baseDelay)
    {
        // Arrange
        var options = new RetryPolicyOptions();

        // Act
        options.BaseDelayMs = baseDelay;

        // Assert
        options.BaseDelayMs.Should().Be(baseDelay);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(60000)]
    public void MaxDelayMs_ShouldAcceptValidValues(int maxDelay)
    {
        // Arrange
        var options = new RetryPolicyOptions();

        // Act
        options.MaxDelayMs = maxDelay;

        // Assert
        options.MaxDelayMs.Should().Be(maxDelay);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void JitterFactor_ShouldAcceptValidValues(double jitterFactor)
    {
        // Arrange
        var options = new RetryPolicyOptions();

        // Act
        options.JitterFactor = jitterFactor;

        // Assert
        options.JitterFactor.Should().Be(jitterFactor);
    }

    [Fact]
    public void UseExponentialBackoff_ShouldBeSettable()
    {
        // Arrange
        var options = new RetryPolicyOptions();

        // Act
        options.UseExponentialBackoff = false;

        // Assert
        options.UseExponentialBackoff.Should().BeFalse();
    }
}
