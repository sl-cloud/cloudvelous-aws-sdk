using Microsoft.Extensions.Logging;

namespace Cloudvelous.Aws.Core;

/// <summary>
/// Base health check implementation for AWS services
/// </summary>
/// <typeparam name="TClient">The AWS client type</typeparam>
public abstract class AwsHealthCheckBase<TClient> : IAwsHealthCheck where TClient : class
{
    /// <summary>
    /// AWS client instance
    /// </summary>
    protected readonly TClient Client;
    
    /// <summary>
    /// Logger instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the AWS health check base
    /// </summary>
    /// <param name="client">AWS client instance</param>
    /// <param name="logger">Logger instance</param>
    protected AwsHealthCheckBase(TClient client, ILogger logger)
    {
        Client = client;
        Logger = logger;
    }

    /// <summary>
    /// Performs the actual health check for the specific AWS service
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy</returns>
    protected abstract Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await PerformHealthCheckAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Health check failed for {ServiceType}", typeof(TClient).Name);
            return false;
        }
    }
}
