using Amazon.Runtime;
using System.Net;

namespace Cloudvelous.Aws.Core;

/// <summary>
/// AWS exception handling utilities
/// </summary>
public static class AwsExceptionHandler
{
    /// <summary>
    /// Determines if an exception is retryable
    /// </summary>
    /// <param name="exception">The exception to check</param>
    /// <returns>True if the exception is retryable</returns>
    public static bool IsRetryableException(Exception exception)
    {
        return exception switch
        {
            AmazonServiceException awsEx => IsRetryableAwsException(awsEx),
            HttpRequestException => true,
            TaskCanceledException => true,
            TimeoutException => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines if an AWS service exception is retryable
    /// </summary>
    /// <param name="exception">The AWS service exception</param>
    /// <returns>True if the exception is retryable</returns>
    public static bool IsRetryableAwsException(AmazonServiceException exception)
    {
        return exception.StatusCode switch
        {
            HttpStatusCode.RequestTimeout => true,
            HttpStatusCode.TooManyRequests => true,
            HttpStatusCode.InternalServerError => true,
            HttpStatusCode.BadGateway => true,
            HttpStatusCode.ServiceUnavailable => true,
            HttpStatusCode.GatewayTimeout => true,
            _ => false
        };
    }

    /// <summary>
    /// Gets a user-friendly error message from an exception
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>User-friendly error message</returns>
    public static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            AmazonServiceException awsEx => $"AWS Service Error: {awsEx.Message} (Error Code: {awsEx.ErrorCode})",
            HttpRequestException httpEx => $"Network Error: {httpEx.Message}",
            TaskCanceledException => "Request was cancelled or timed out",
            TimeoutException => "Request timed out",
            _ => $"Unexpected error: {exception.Message}"
        };
    }
}
