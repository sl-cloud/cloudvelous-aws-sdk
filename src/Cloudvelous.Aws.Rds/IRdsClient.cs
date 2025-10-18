using Amazon.RDS.Model;
using System.Data.SqlClient;

namespace Cloudvelous.Aws.Rds;

/// <summary>
/// RDS client interface
/// </summary>
public interface IRdsClient
{
    /// <summary>
    /// Gets RDS instance information
    /// </summary>
    /// <param name="dbInstanceIdentifier">DB instance identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>DB instance information</returns>
    Task<DBInstance> GetDbInstanceAsync(string dbInstanceIdentifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists RDS instances
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of DB instances</returns>
    Task<List<DBInstance>> ListDbInstancesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an IAM authentication token for RDS
    /// </summary>
    /// <param name="hostname">RDS hostname</param>
    /// <param name="port">RDS port</param>
    /// <param name="username">Database username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IAM authentication token</returns>
    Task<string> GenerateAuthTokenAsync(string hostname, int port, string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets connection information for an RDS instance with IAM authentication
    /// </summary>
    /// <param name="dbInstanceIdentifier">DB instance identifier</param>
    /// <param name="databaseName">Database name</param>
    /// <param name="username">Database username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Connection information</returns>
    Task<RdsConnectionInfo> GetConnectionInfoAsync(string dbInstanceIdentifier, string databaseName, string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds a connection string for RDS with IAM authentication
    /// </summary>
    /// <param name="connectionInfo">Connection information</param>
    /// <param name="additionalOptions">Additional connection string options</param>
    /// <returns>Connection string</returns>
    string BuildConnectionString(RdsConnectionInfo connectionInfo, Dictionary<string, string>? additionalOptions = null);

    /// <summary>
    /// Creates a SQL connection with IAM authentication
    /// </summary>
    /// <param name="connectionInfo">Connection information</param>
    /// <param name="additionalOptions">Additional connection string options</param>
    /// <returns>SQL connection</returns>
    SqlConnection CreateConnection(RdsConnectionInfo connectionInfo, Dictionary<string, string>? additionalOptions = null);

    /// <summary>
    /// Tests the connection to an RDS instance
    /// </summary>
    /// <param name="connectionInfo">Connection information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if connection is successful</returns>
    Task<bool> TestConnectionAsync(RdsConnectionInfo connectionInfo, CancellationToken cancellationToken = default);
}
