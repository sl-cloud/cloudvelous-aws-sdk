using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Rds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Cloudvelous.Aws.Rds.Extensions;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds RDS services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Options configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddRds(this IServiceCollection services, Action<RdsOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<IRdsClient, RdsClient>();
        services.AddSingleton<IAwsHealthCheck, RdsHealthCheck>();
        
        return services;
    }

}