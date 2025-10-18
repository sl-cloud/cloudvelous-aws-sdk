using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Rds;
using Microsoft.Extensions.Logging;

namespace Cloudvelous.Aws.Rds.Extensions;

/// <summary>
/// RDS health check implementation
/// </summary>
public class RdsHealthCheck : AwsHealthCheckBase<IRdsClient>
{
    /// <summary>
    /// Initializes a new instance of the RDS health check
    /// </summary>
    /// <param name="client">RDS client instance</param>
    /// <param name="logger">Logger instance</param>
    public RdsHealthCheck(IRdsClient client, ILogger<RdsHealthCheck> logger) 
        : base(client, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Try to list DB instances as a health check
            await Client.ListDbInstancesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
