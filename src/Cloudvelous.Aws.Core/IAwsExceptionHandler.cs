namespace Cloudvelous.Aws.Core;

/// <summary>
/// AWS exception handler service
/// </summary>
public interface IAwsExceptionHandler
{
    /// <summary>
    /// Handles an AWS exception
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="context">Additional context</param>
    /// <returns>Handling result</returns>
    Task<ExceptionHandlingResult> HandleExceptionAsync(Exception exception, string? context = null);
}
