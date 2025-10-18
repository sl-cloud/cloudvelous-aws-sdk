using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.SecretsManager;
using Microsoft.Extensions.Logging;

namespace Cloudvelous.Aws.SecretsManager.Extensions;

/// <summary>
/// Secrets Manager health check implementation
/// </summary>
public class SecretsManagerHealthCheck : AwsHealthCheckBase<ISecretsManagerClient>
{
    /// <summary>
    /// Initializes a new instance of the Secrets Manager health check
    /// </summary>
    /// <param name="client">Secrets Manager client instance</param>
    /// <param name="logger">Logger instance</param>
    public SecretsManagerHealthCheck(ISecretsManagerClient client, ILogger<SecretsManagerHealthCheck> logger) 
        : base(client, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Try to list secrets as a health check
            await Client.ListSecretsAsync(1, null, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
