using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Rds;
using FluentAssertions;

namespace Cloudvelous.Aws.Rds.Tests;

public class RdsOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new RdsOptions();

        // Assert
        options.Region.Should().Be("us-east-1");
        options.DefaultPort.Should().Be(1433);
        options.DefaultConnectionTimeoutSeconds.Should().Be(30);
        options.DefaultCommandTimeoutSeconds.Should().Be(30);
        options.UseSsl.Should().BeTrue();
        options.SslCaFile.Should().BeNull();
    }

    [Theory]
    [InlineData(1433)]
    [InlineData(3306)]
    [InlineData(5432)]
    public void DefaultPort_ShouldAcceptValidValues(int port)
    {
        // Arrange
        var options = new RdsOptions();

        // Act
        options.DefaultPort = port;

        // Assert
        options.DefaultPort.Should().Be(port);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(60)]
    public void DefaultConnectionTimeoutSeconds_ShouldAcceptValidValues(int timeout)
    {
        // Arrange
        var options = new RdsOptions();

        // Act
        options.DefaultConnectionTimeoutSeconds = timeout;

        // Assert
        options.DefaultConnectionTimeoutSeconds.Should().Be(timeout);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(60)]
    public void DefaultCommandTimeoutSeconds_ShouldAcceptValidValues(int timeout)
    {
        // Arrange
        var options = new RdsOptions();

        // Act
        options.DefaultCommandTimeoutSeconds = timeout;

        // Assert
        options.DefaultCommandTimeoutSeconds.Should().Be(timeout);
    }

    [Fact]
    public void UseSsl_ShouldBeSettable()
    {
        // Arrange
        var options = new RdsOptions();

        // Act
        options.UseSsl = false;

        // Assert
        options.UseSsl.Should().BeFalse();
    }

    [Fact]
    public void SslCaFile_ShouldBeSettable()
    {
        // Arrange
        var options = new RdsOptions();
        var caFile = "/path/to/ca.pem";

        // Act
        options.SslCaFile = caFile;

        // Assert
        options.SslCaFile.Should().Be(caFile);
    }

    [Fact]
    public void RdsOptions_ShouldInheritFromAwsClientOptions()
    {
        // Act
        var options = new RdsOptions();

        // Assert
        options.Should().BeAssignableTo<AwsClientOptions>();
    }
}
