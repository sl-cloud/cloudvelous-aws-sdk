using Cloudvelous.Aws.Core;
using FluentAssertions;

namespace Cloudvelous.Aws.Core.Tests;

public class CircuitBreakerOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new CircuitBreakerOptions();

        // Assert
        options.FailureThreshold.Should().Be(5);
        options.DurationOfBreakSeconds.Should().Be(30);
        options.SamplingDurationSeconds.Should().Be(10);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void FailureThreshold_ShouldAcceptValidValues(int threshold)
    {
        // Arrange
        var options = new CircuitBreakerOptions();

        // Act
        options.FailureThreshold = threshold;

        // Assert
        options.FailureThreshold.Should().Be(threshold);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(60)]
    public void DurationOfBreakSeconds_ShouldAcceptValidValues(int duration)
    {
        // Arrange
        var options = new CircuitBreakerOptions();

        // Act
        options.DurationOfBreakSeconds = duration;

        // Assert
        options.DurationOfBreakSeconds.Should().Be(duration);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(30)]
    public void SamplingDurationSeconds_ShouldAcceptValidValues(int duration)
    {
        // Arrange
        var options = new CircuitBreakerOptions();

        // Act
        options.SamplingDurationSeconds = duration;

        // Assert
        options.SamplingDurationSeconds.Should().Be(duration);
    }
}
