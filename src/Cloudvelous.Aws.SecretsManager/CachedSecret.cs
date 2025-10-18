namespace Cloudvelous.Aws.SecretsManager;

/// <summary>
/// Cached secret information
/// </summary>
public class CachedSecret
{
    /// <summary>
    /// Secret value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Secret version ID
    /// </summary>
    public string VersionId { get; set; } = string.Empty;

    /// <summary>
    /// Secret ARN
    /// </summary>
    public string Arn { get; set; } = string.Empty;

    /// <summary>
    /// Secret name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Cached timestamp
    /// </summary>
    public DateTime CachedAt { get; set; }

    /// <summary>
    /// Secret creation date
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Secret last modified date
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }
}
