using Amazon;
using Amazon.RDS;
using Amazon.RDS.Model;
using Amazon.RDS.Util;
using Cloudvelous.Aws.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace Cloudvelous.Aws.Rds;

/// <summary>
/// RDS client implementation
/// </summary>
public class RdsClient : AwsClientFactoryBase<AmazonRDSClient, RdsOptions>, IRdsClient
{
    private readonly IAmazonRDS _rdsClient;

    /// <summary>
    /// Initializes a new instance of the RDS client
    /// </summary>
    /// <param name="options">RDS configuration options</param>
    /// <param name="logger">Logger instance</param>
    public RdsClient(IOptions<RdsOptions> options, ILogger<RdsClient> logger) : base(options, logger)
    {
        _rdsClient = CreateClient();
    }

    /// <summary>
    /// Initializes a new instance of the RDS client with a specific client instance (for testing)
    /// </summary>
    /// <param name="options">RDS configuration options</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="rdsClient">RDS client instance</param>
    internal RdsClient(IOptions<RdsOptions> options, ILogger<RdsClient> logger, IAmazonRDS rdsClient) : base(options, logger)
    {
        _rdsClient = rdsClient;
    }

    /// <inheritdoc />
    protected override Amazon.Runtime.ClientConfig CreateSpecificClientConfig()
    {
        return new AmazonRDSConfig();
    }

    /// <inheritdoc />
    protected override AmazonRDSClient CreateClientInstance(Amazon.Runtime.ClientConfig config)
    {
        return new AmazonRDSClient((AmazonRDSConfig)config);
    }

    /// <inheritdoc />
    public async Task<DBInstance> GetDbInstanceAsync(string dbInstanceIdentifier, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting RDS instance: {DbInstanceIdentifier}", dbInstanceIdentifier);

        var request = new DescribeDBInstancesRequest
        {
            DBInstanceIdentifier = dbInstanceIdentifier
        };

        var response = await _rdsClient.DescribeDBInstancesAsync(request, cancellationToken);
        return response.DBInstances.FirstOrDefault() ?? throw new InvalidOperationException($"RDS instance '{dbInstanceIdentifier}' not found.");
    }

    /// <inheritdoc />
    public async Task<List<DBInstance>> ListDbInstancesAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing RDS instances");

        var request = new DescribeDBInstancesRequest();
        var response = await _rdsClient.DescribeDBInstancesAsync(request, cancellationToken);
        return response.DBInstances;
    }

    /// <inheritdoc />
    public Task<string> GenerateAuthTokenAsync(string hostname, int port, string username, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Generating IAM auth token for RDS instance: {Hostname}:{Port} for user: {Username}", hostname, port, username);
        var regionEndpoint = RegionEndpoint.GetBySystemName(Options.Region);
        var authToken = RDSAuthTokenGenerator.GenerateAuthToken(regionEndpoint, hostname, port, username);
        return Task.FromResult(authToken);
    }

    /// <inheritdoc />
    public async Task<RdsConnectionInfo> GetConnectionInfoAsync(string dbInstanceIdentifier, string databaseName, string username, CancellationToken cancellationToken = default)
    {
        var dbInstance = await GetDbInstanceAsync(dbInstanceIdentifier, cancellationToken);
        var port = dbInstance.Endpoint.Port ?? Options.DefaultPort;
        var authToken = await GenerateAuthTokenAsync(dbInstance.Endpoint.Address, port, username, cancellationToken);

        return new RdsConnectionInfo
        {
            Hostname = dbInstance.Endpoint.Address,
            Port = port,
            DatabaseName = databaseName,
            Username = username,
            AuthToken = authToken,
            TokenExpiration = DateTime.UtcNow.AddMinutes(14) // IAM tokens are valid for 15 minutes
        };
    }

    /// <inheritdoc />
    public string BuildConnectionString(RdsConnectionInfo connectionInfo, Dictionary<string, string>? additionalOptions = null)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = $"{connectionInfo.Hostname},{connectionInfo.Port}",
            InitialCatalog = connectionInfo.DatabaseName,
            UserID = connectionInfo.Username,
            Password = connectionInfo.AuthToken,
            Encrypt = Options.UseSsl,
            TrustServerCertificate = !Options.ValidateServerCertificate,
            IntegratedSecurity = false,
            Pooling = true,
            ConnectTimeout = Options.DefaultConnectionTimeoutSeconds
        };

        if (additionalOptions != null)
        {
            foreach (var option in additionalOptions)
            {
                try
                {
                    builder.Add(option.Key, option.Value);
                }
                catch (ArgumentException ex)
                {
                    // Skip invalid connection string keywords
                    Logger.LogWarning(ex, "Skipping invalid connection string keyword: {Keyword}", option.Key);
                }
            }
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public SqlConnection CreateConnection(RdsConnectionInfo connectionInfo, Dictionary<string, string>? additionalOptions = null)
    {
        var connectionString = BuildConnectionString(connectionInfo, additionalOptions);
        return new SqlConnection(connectionString);
    }

    /// <inheritdoc />
    public async Task<bool> TestConnectionAsync(RdsConnectionInfo connectionInfo, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection(connectionInfo);
        try
        {
            await connection.OpenAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to connect to RDS instance: {Hostname}:{Port}", connectionInfo.Hostname, connectionInfo.Port);
            return false;
        }
    }
}