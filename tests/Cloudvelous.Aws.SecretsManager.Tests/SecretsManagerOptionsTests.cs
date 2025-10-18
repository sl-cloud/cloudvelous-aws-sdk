using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.SecretsManager;
using FluentAssertions;

namespace Cloudvelous.Aws.SecretsManager.Tests;

public class SecretsManagerOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new SecretsManagerOptions();

        // Assert
        options.Region.Should().Be("us-east-1");
        options.DefaultCacheDurationMinutes.Should().Be(60);
        options.MaxCacheSize.Should().Be(1000);
        options.EnableCaching.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(60)]
    [InlineData(1440)]
    public void DefaultCacheDurationMinutes_ShouldAcceptValidValues(int duration)
    {
        // Arrange
        var options = new SecretsManagerOptions();

        // Act
        options.DefaultCacheDurationMinutes = duration;

        // Assert
        options.DefaultCacheDurationMinutes.Should().Be(duration);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void MaxCacheSize_ShouldAcceptValidValues(int maxSize)
    {
        // Arrange
        var options = new SecretsManagerOptions();

        // Act
        options.MaxCacheSize = maxSize;

        // Assert
        options.MaxCacheSize.Should().Be(maxSize);
    }

    [Fact]
    public void EnableCaching_ShouldBeSettable()
    {
        // Arrange
        var options = new SecretsManagerOptions();

        // Act
        options.EnableCaching = false;

        // Assert
        options.EnableCaching.Should().BeFalse();
    }

    [Fact]
    public void SecretsManagerOptions_ShouldInheritFromAwsClientOptions()
    {
        // Act
        var options = new SecretsManagerOptions();

        // Assert
        options.Should().BeAssignableTo<AwsClientOptions>();
    }
}
