using Amazon.Runtime;
using Cloudvelous.Aws.Core;
using FluentAssertions;
using System.Net;

namespace Cloudvelous.Aws.Core.Tests;

public class AwsExceptionHandlerTests
{
    [Fact]
    public void IsRetryableException_WithHttpRequestException_ShouldReturnTrue()
    {
        // Arrange
        var exception = new HttpRequestException("Network error");

        // Act
        var result = AwsExceptionHandler.IsRetryableException(exception);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsRetryableException_WithTaskCanceledException_ShouldReturnTrue()
    {
        // Arrange
        var exception = new TaskCanceledException("Operation was canceled");

        // Act
        var result = AwsExceptionHandler.IsRetryableException(exception);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsRetryableException_WithTimeoutException_ShouldReturnTrue()
    {
        // Arrange
        var exception = new TimeoutException("Operation timed out");

        // Act
        var result = AwsExceptionHandler.IsRetryableException(exception);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(HttpStatusCode.RequestTimeout)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    public void IsRetryableAwsException_WithRetryableStatusCodes_ShouldReturnTrue(HttpStatusCode statusCode)
    {
        // Arrange
        var exception = new AmazonServiceException($"Error with status {statusCode}")
        {
            StatusCode = statusCode
        };

        // Act
        var result = AwsExceptionHandler.IsRetryableAwsException(exception);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    public void IsRetryableAwsException_WithNonRetryableStatusCodes_ShouldReturnFalse(HttpStatusCode statusCode)
    {
        // Arrange
        var exception = new AmazonServiceException($"Error with status {statusCode}")
        {
            StatusCode = statusCode
        };

        // Act
        var result = AwsExceptionHandler.IsRetryableAwsException(exception);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsRetryableException_WithAmazonServiceException_ShouldDelegateToIsRetryableAwsException()
    {
        // Arrange
        var exception = new AmazonServiceException("AWS service error")
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        // Act
        var result = AwsExceptionHandler.IsRetryableException(exception);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsRetryableException_WithGenericException_ShouldReturnFalse()
    {
        // Arrange
        var exception = new InvalidOperationException("Generic error");

        // Act
        var result = AwsExceptionHandler.IsRetryableException(exception);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetUserFriendlyMessage_WithAmazonServiceException_ShouldReturnFormattedMessage()
    {
        // Arrange
        var exception = new AmazonServiceException("Service error")
        {
            ErrorCode = "ServiceUnavailable"
        };

        // Act
        var result = AwsExceptionHandler.GetUserFriendlyMessage(exception);

        // Assert
        result.Should().Be("AWS Service Error: Service error (Error Code: ServiceUnavailable)");
    }

    [Fact]
    public void GetUserFriendlyMessage_WithHttpRequestException_ShouldReturnFormattedMessage()
    {
        // Arrange
        var exception = new HttpRequestException("Network error");

        // Act
        var result = AwsExceptionHandler.GetUserFriendlyMessage(exception);

        // Assert
        result.Should().Be("Network Error: Network error");
    }

    [Fact]
    public void GetUserFriendlyMessage_WithTaskCanceledException_ShouldReturnFormattedMessage()
    {
        // Arrange
        var exception = new TaskCanceledException("Operation was canceled");

        // Act
        var result = AwsExceptionHandler.GetUserFriendlyMessage(exception);

        // Assert
        result.Should().Be("Request was cancelled or timed out");
    }

    [Fact]
    public void GetUserFriendlyMessage_WithTimeoutException_ShouldReturnFormattedMessage()
    {
        // Arrange
        var exception = new TimeoutException("Operation timed out");

        // Act
        var result = AwsExceptionHandler.GetUserFriendlyMessage(exception);

        // Assert
        result.Should().Be("Request timed out");
    }

    [Fact]
    public void GetUserFriendlyMessage_WithGenericException_ShouldReturnFormattedMessage()
    {
        // Arrange
        var exception = new InvalidOperationException("Generic error");

        // Act
        var result = AwsExceptionHandler.GetUserFriendlyMessage(exception);

        // Assert
        result.Should().Be("Unexpected error: Generic error");
    }
}
