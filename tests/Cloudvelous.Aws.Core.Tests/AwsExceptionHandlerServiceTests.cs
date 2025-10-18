using Cloudvelous.Aws.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cloudvelous.Aws.Core.Tests;

public class AwsExceptionHandlerServiceTests
{
    private readonly Mock<ILogger<AwsExceptionHandlerService>> _mockLogger;
    private readonly AwsExceptionHandlerService _service;

    public AwsExceptionHandlerServiceTests()
    {
        _mockLogger = new Mock<ILogger<AwsExceptionHandlerService>>();
        _service = new AwsExceptionHandlerService(_mockLogger.Object);
    }

    [Fact]
    public async Task HandleExceptionAsync_WithRetryableException_ShouldReturnShouldRetryTrue()
    {
        // Arrange
        var exception = new HttpRequestException("Network error");
        var context = "Test context";

        // Act
        var result = await _service.HandleExceptionAsync(exception, context);

        // Assert
        result.Should().NotBeNull();
        result.Handled.Should().BeTrue();
        result.ShouldRetry.Should().BeTrue();
        result.UserMessage.Should().Be("Network Error: Network error");
        result.LogContext.Should().Be(context);
    }

    [Fact]
    public async Task HandleExceptionAsync_WithNonRetryableException_ShouldReturnShouldRetryFalse()
    {
        // Arrange
        var exception = new InvalidOperationException("Non-retryable error");
        var context = "Test context";

        // Act
        var result = await _service.HandleExceptionAsync(exception, context);

        // Assert
        result.Should().NotBeNull();
        result.Handled.Should().BeTrue();
        result.ShouldRetry.Should().BeFalse();
        result.UserMessage.Should().Be("Unexpected error: Non-retryable error");
        result.LogContext.Should().Be(context);
    }

    [Fact]
    public async Task HandleExceptionAsync_WithNullContext_ShouldHandleGracefully()
    {
        // Arrange
        var exception = new HttpRequestException("Network error");

        // Act
        var result = await _service.HandleExceptionAsync(exception, null);

        // Assert
        result.Should().NotBeNull();
        result.Handled.Should().BeTrue();
        result.LogContext.Should().BeNull();
    }

    [Fact]
    public async Task HandleExceptionAsync_WithRetryableException_ShouldLogWarning()
    {
        // Arrange
        var exception = new HttpRequestException("Network error");
        var context = "Test context";

        // Act
        await _service.HandleExceptionAsync(exception, context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retryable AWS exception occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleExceptionAsync_WithNonRetryableException_ShouldLogError()
    {
        // Arrange
        var exception = new InvalidOperationException("Non-retryable error");
        var context = "Test context";

        // Act
        await _service.HandleExceptionAsync(exception, context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Non-retryable AWS exception occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
