namespace Cloudvelous.Aws.Rds;

/// <summary>
/// RDS connection information
/// </summary>
public class RdsConnectionInfo
{
    /// <summary>
    /// Database hostname
    /// </summary>
    public string Hostname { get; set; } = string.Empty;

    /// <summary>
    /// Database port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Database name
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>
    /// Database username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// IAM authentication token
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime TokenExpiration { get; set; }
}
