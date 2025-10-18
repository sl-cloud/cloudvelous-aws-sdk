namespace Cloudvelous.Aws.Core;

/// <summary>
/// Retry policy configuration options
/// </summary>
public class RetryPolicyOptions
{
    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Base delay between retries in milliseconds
    /// </summary>
    public int BaseDelayMs { get; set; } = 1000;

    /// <summary>
    /// Maximum delay between retries in milliseconds
    /// </summary>
    public int MaxDelayMs { get; set; } = 10000;

    /// <summary>
    /// Whether to use exponential backoff
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Jitter factor for retry delays (0.0 to 1.0)
    /// </summary>
    public double JitterFactor { get; set; } = 0.1;
}
