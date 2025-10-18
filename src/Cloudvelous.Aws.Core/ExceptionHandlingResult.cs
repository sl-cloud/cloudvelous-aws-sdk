namespace Cloudvelous.Aws.Core;

/// <summary>
/// Exception handling result
/// </summary>
public class ExceptionHandlingResult
{
    /// <summary>
    /// Whether the exception was handled
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    /// Whether the operation should be retried
    /// </summary>
    public bool ShouldRetry { get; set; }

    /// <summary>
    /// User-friendly error message
    /// </summary>
    public string? UserMessage { get; set; }

    /// <summary>
    /// Additional context for logging
    /// </summary>
    public string? LogContext { get; set; }
}
