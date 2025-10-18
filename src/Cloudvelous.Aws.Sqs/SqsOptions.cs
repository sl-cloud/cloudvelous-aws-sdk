using Cloudvelous.Aws.Core;

namespace Cloudvelous.Aws.Sqs;

/// <summary>
/// SQS-specific configuration options
/// </summary>
public class SqsOptions : AwsClientOptions
{
    /// <summary>
    /// Default visibility timeout in seconds
    /// </summary>
    public int DefaultVisibilityTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Default message retention period in seconds
    /// </summary>
    public int DefaultMessageRetentionPeriodSeconds { get; set; } = 1209600; // 14 days

    /// <summary>
    /// Default receive message wait time in seconds (for long polling)
    /// </summary>
    public int DefaultReceiveMessageWaitTimeSeconds { get; set; } = 20;

    /// <summary>
    /// Maximum number of messages to receive in a single call
    /// </summary>
    public int MaxReceiveMessages { get; set; } = 10;
}
