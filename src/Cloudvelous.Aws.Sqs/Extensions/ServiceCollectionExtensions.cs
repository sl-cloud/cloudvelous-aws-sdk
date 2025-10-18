using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Sqs;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudvelous.Aws.Sqs.Extensions;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQS services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Options configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSqs(this IServiceCollection services, Action<SqsOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<ISqsClient, SqsClient>();
        services.AddSingleton<IAwsHealthCheck, SqsHealthCheck>();
        
        return services;
    }
}