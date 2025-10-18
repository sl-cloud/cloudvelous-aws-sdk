namespace Cloudvelous.Aws.Core;

/// <summary>
/// Circuit breaker configuration options
/// </summary>
public class CircuitBreakerOptions
{
    /// <summary>
    /// Number of consecutive failures before opening the circuit
    /// </summary>
    public int FailureThreshold { get; set; } = 5;

    /// <summary>
    /// Duration in seconds the circuit stays open
    /// </summary>
    public int DurationOfBreakSeconds { get; set; } = 30;

    /// <summary>
    /// Number of successful calls needed to close the circuit
    /// </summary>
    public int SamplingDurationSeconds { get; set; } = 10;
}
