using Cloudvelous.Aws.Core;

namespace Cloudvelous.Aws.Rds;

/// <summary>
/// RDS-specific configuration options
/// </summary>
public class RdsOptions : AwsClientOptions
{
    /// <summary>
    /// Default database port
    /// </summary>
    public int DefaultPort { get; set; } = 1433;

    /// <summary>
    /// Default connection timeout in seconds
    /// </summary>
    public int DefaultConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Default command timeout in seconds
    /// </summary>
    public int DefaultCommandTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Whether to use SSL for connections
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// Whether to validate server certificate
        /// </summary>
        public bool ValidateServerCertificate { get; set; } = true;

        /// <summary>
        /// SSL certificate authority file path (optional)
        /// </summary>
        public string? SslCaFile { get; set; }
}
