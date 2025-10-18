using Microsoft.Extensions.Logging;

namespace Cloudvelous.Aws.Core;

/// <summary>
/// AWS exception handler service implementation
/// </summary>
public class AwsExceptionHandlerService : IAwsExceptionHandler
{
    private readonly ILogger<AwsExceptionHandlerService> _logger;

    /// <summary>
    /// Initializes a new instance of the AWS exception handler service
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public AwsExceptionHandlerService(ILogger<AwsExceptionHandlerService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ExceptionHandlingResult> HandleExceptionAsync(Exception exception, string? context = null)
    {
        var result = new ExceptionHandlingResult
        {
            Handled = true,
            ShouldRetry = AwsExceptionHandler.IsRetryableException(exception),
            UserMessage = AwsExceptionHandler.GetUserFriendlyMessage(exception),
            LogContext = context
        };

        if (result.ShouldRetry)
        {
            _logger.LogWarning(exception, "Retryable AWS exception occurred. Context: {Context}", context);
        }
        else
        {
            _logger.LogError(exception, "Non-retryable AWS exception occurred. Context: {Context}", context);
        }

        return Task.FromResult(result);
    }
}
