using Microsoft.Extensions.DependencyInjection;

namespace Cloudvelous.Aws.Core;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AWS core services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Options configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddAwsCore(this IServiceCollection services, Action<AwsClientOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<IAwsExceptionHandler, AwsExceptionHandlerService>();
        
        return services;
    }
}
