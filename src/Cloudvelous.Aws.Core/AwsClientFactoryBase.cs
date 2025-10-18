using Amazon;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cloudvelous.Aws.Core;

/// <summary>
/// Base implementation for AWS client factory
/// </summary>
/// <typeparam name="TClient">The AWS client type</typeparam>
/// <typeparam name="TOptions">The options type</typeparam>
public abstract class AwsClientFactoryBase<TClient, TOptions> : IAwsClientFactory<TClient>
    where TClient : class
    where TOptions : AwsClientOptions
{
    /// <summary>
    /// Configuration options for the AWS client
    /// </summary>
    protected readonly TOptions Options;
    
    /// <summary>
    /// Logger instance for the AWS client
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the AWS client factory base
    /// </summary>
    /// <param name="options">Configuration options</param>
    /// <param name="logger">Logger instance</param>
    protected AwsClientFactoryBase(IOptions<TOptions> options, ILogger logger)
    {
        Options = options.Value;
        Logger = logger;
    }

    /// <summary>
    /// Creates the base AWS client configuration
    /// </summary>
    /// <returns>AWS client configuration</returns>
    protected virtual Amazon.Runtime.ClientConfig CreateClientConfig()
    {
        var config = CreateSpecificClientConfig();
        
        if (!string.IsNullOrEmpty(Options.Region))
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(Options.Region);
        }

        config.Timeout = TimeSpan.FromSeconds(Options.RequestTimeoutSeconds);
        
        return config;
    }

    /// <summary>
    /// Creates the specific AWS client configuration
    /// </summary>
    /// <returns>Specific AWS client configuration</returns>
    protected abstract Amazon.Runtime.ClientConfig CreateSpecificClientConfig();

    /// <summary>
    /// Creates the AWS client instance
    /// </summary>
    /// <param name="config">AWS client configuration</param>
    /// <returns>AWS client instance</returns>
    protected abstract TClient CreateClientInstance(Amazon.Runtime.ClientConfig config);

    /// <inheritdoc />
    public virtual TClient CreateClient()
    {
        var config = CreateClientConfig();
        return CreateClientInstance(config);
    }
}
