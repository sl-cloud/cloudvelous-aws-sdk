using Amazon;

namespace Cloudvelous.Aws.Core;

/// <summary>
/// Base configuration options for AWS clients
/// </summary>
public class AwsClientOptions
{
    /// <summary>
    /// AWS region endpoint
    /// </summary>
    public string Region { get; set; } = RegionEndpoint.USEast1.SystemName;

    /// <summary>
    /// AWS access key ID (optional - can use IAM roles, environment variables, etc.)
    /// </summary>
    public string? AccessKeyId { get; set; }

    /// <summary>
    /// AWS secret access key (optional - can use IAM roles, environment variables, etc.)
    /// </summary>
    public string? SecretAccessKey { get; set; }

    /// <summary>
    /// AWS session token (optional - for temporary credentials)
    /// </summary>
    public string? SessionToken { get; set; }

    /// <summary>
    /// Retry policy configuration
    /// </summary>
    public RetryPolicyOptions RetryPolicy { get; set; } = new();

    /// <summary>
    /// Circuit breaker configuration
    /// </summary>
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 30;
}