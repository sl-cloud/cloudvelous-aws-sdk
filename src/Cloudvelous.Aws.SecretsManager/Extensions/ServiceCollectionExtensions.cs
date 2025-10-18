using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.SecretsManager;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Cloudvelous.Aws.SecretsManager.Extensions;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Secrets Manager services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Options configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSecretsManager(this IServiceCollection services, Action<SecretsManagerOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddMemoryCache();
        services.AddSingleton<ISecretsManagerClient, SecretsManagerClient>();
        services.AddSingleton<IAwsHealthCheck, SecretsManagerHealthCheck>();
        
        return services;
    }

}