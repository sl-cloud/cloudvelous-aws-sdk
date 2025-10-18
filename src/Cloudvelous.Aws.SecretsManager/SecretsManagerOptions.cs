using Cloudvelous.Aws.Core;

namespace Cloudvelous.Aws.SecretsManager;

/// <summary>
/// Secrets Manager-specific configuration options
/// </summary>
public class SecretsManagerOptions : AwsClientOptions
{
    /// <summary>
    /// Default cache duration in minutes
    /// </summary>
    public int DefaultCacheDurationMinutes { get; set; } = 60;

    /// <summary>
    /// Maximum cache size
    /// </summary>
    public int MaxCacheSize { get; set; } = 1000;

    /// <summary>
    /// Whether to enable caching by default
    /// </summary>
    public bool EnableCaching { get; set; } = true;
}
