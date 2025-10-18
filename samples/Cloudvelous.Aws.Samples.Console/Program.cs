using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Rds;
using Cloudvelous.Aws.Rds.Extensions;
using Cloudvelous.Aws.SecretsManager;
using Cloudvelous.Aws.SecretsManager.Extensions;
using Cloudvelous.Aws.Sqs;
using Cloudvelous.Aws.Sqs.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cloudvelous.Aws.Samples.Console;

/// <summary>
/// Sample console application demonstrating AWS SDK usage
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Starting Cloudvelous AWS SDK Sample Application");
            
            // Demonstrate SQS operations
            await DemonstrateSqsOperations(scope.ServiceProvider, logger);
            
            // Demonstrate Secrets Manager operations
            await DemonstrateSecretsManagerOperations(scope.ServiceProvider, logger);
            
            // Demonstrate RDS operations
            await DemonstrateRdsOperations(scope.ServiceProvider, logger);
            
            logger.LogInformation("Sample application completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during sample execution");
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true)
                      .AddEnvironmentVariables()
                      .AddCommandLine(args);
            })
            .ConfigureServices((context, services) =>
            {
                // Add AWS Core services
                services.AddAwsCore(options =>
                {
                    options.Region = context.Configuration["AWS:Region"] ?? "us-east-1";
                    options.RetryPolicy.MaxRetryAttempts = 3;
                    options.CircuitBreaker.FailureThreshold = 5;
                });

                // Add AWS service clients
                services.AddSqs(options =>
                {
                    options.DefaultVisibilityTimeoutSeconds = 30;
                    options.MaxReceiveMessages = 10;
                });

                services.AddSecretsManager(options =>
                {
                    options.DefaultCacheDurationMinutes = 60;
                    options.EnableCaching = true;
                });

                services.AddRds(options =>
                {
                    options.DefaultPort = 1433;
                    options.UseSsl = true;
                });

                // Add sample service
                services.AddScoped<SampleService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

    private static async Task DemonstrateSqsOperations(IServiceProvider serviceProvider, ILogger logger)
    {
        logger.LogInformation("=== SQS Operations Demo ===");
        
        var sqsClient = serviceProvider.GetRequiredService<ISqsClient>();
        
        try
        {
            // Create a test queue
            var queueName = "test-queue-" + Guid.NewGuid().ToString("N")[..8];
            var queueUrl = await sqsClient.CreateQueueAsync(queueName);
            logger.LogInformation("Created queue: {QueueUrl}", queueUrl);

            // Send a message
            var message = new { Text = "Hello from Cloudvelous AWS SDK!", Timestamp = DateTime.UtcNow };
            var messageId = await sqsClient.SendMessageAsync(queueUrl, message);
            logger.LogInformation("Sent message with ID: {MessageId}", messageId);

            // Receive messages
            var messages = await sqsClient.ReceiveMessagesAsync<object>(queueUrl);
            logger.LogInformation("Received {Count} messages", messages.Count);

            // Delete messages
            foreach (var msg in messages)
            {
                await sqsClient.DeleteMessageAsync(queueUrl, msg.ReceiptHandle);
                logger.LogInformation("Deleted message: {MessageId}", msg.MessageId);
            }

            // Clean up
            await sqsClient.DeleteQueueAsync(queueUrl);
            logger.LogInformation("Deleted queue: {QueueUrl}", queueUrl);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SQS operations failed");
        }
    }

    private static async Task DemonstrateSecretsManagerOperations(IServiceProvider serviceProvider, ILogger logger)
    {
        logger.LogInformation("=== Secrets Manager Operations Demo ===");
        
        var secretsClient = serviceProvider.GetRequiredService<ISecretsManagerClient>();
        
        try
        {
            // Create a test secret
            var secretName = "test-secret-" + Guid.NewGuid().ToString("N")[..8];
            var secretValue = new { ApiKey = "test-api-key", DatabaseUrl = "test-db-url" };
            
            var secretArn = await secretsClient.CreateSecretAsync(secretName, secretValue, "Test secret for demo");
            logger.LogInformation("Created secret: {SecretArn}", secretArn);

            // Retrieve the secret
            var retrievedSecret = await secretsClient.GetSecretValueAsync<object>(secretName);
            logger.LogInformation("Retrieved secret: {SecretName}", secretName);

            // Update the secret
            var updatedValue = new { ApiKey = "updated-api-key", DatabaseUrl = "updated-db-url" };
            await secretsClient.UpdateSecretAsync(secretName, updatedValue);
            logger.LogInformation("Updated secret: {SecretName}", secretName);

            // Clean up
            await secretsClient.DeleteSecretAsync(secretName, forceDeleteWithoutRecovery: true);
            logger.LogInformation("Deleted secret: {SecretName}", secretName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Secrets Manager operations failed");
        }
    }

    private static async Task DemonstrateRdsOperations(IServiceProvider serviceProvider, ILogger logger)
    {
        logger.LogInformation("=== RDS Operations Demo ===");
        
        var rdsClient = serviceProvider.GetRequiredService<IRdsClient>();
        
        try
        {
            // List DB instances
            var dbInstances = await rdsClient.ListDbInstancesAsync();
            logger.LogInformation("Found {Count} DB instances", dbInstances.Count);

            foreach (var instance in dbInstances.Take(3)) // Show first 3 instances
            {
                logger.LogInformation("DB Instance: {Identifier} - {Engine} - {Status}", 
                    instance.DBInstanceIdentifier, instance.Engine, instance.DBInstanceStatus);
            }

            // Note: IAM token generation requires actual RDS instances
            // This is just a demonstration of the API structure
            logger.LogInformation("RDS operations completed (IAM token generation requires actual RDS instances)");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RDS operations failed");
        }
    }
}

/// <summary>
/// Sample service demonstrating dependency injection
/// </summary>
public class SampleService
{
    private readonly ILogger<SampleService> _logger;
    private readonly ISqsClient _sqsClient;
    private readonly ISecretsManagerClient _secretsClient;
    private readonly IRdsClient _rdsClient;

    public SampleService(
        ILogger<SampleService> logger,
        ISqsClient sqsClient,
        ISecretsManagerClient secretsClient,
        IRdsClient rdsClient)
    {
        _logger = logger;
        _sqsClient = sqsClient;
        _secretsClient = secretsClient;
        _rdsClient = rdsClient;
    }

    public async Task RunSampleAsync()
    {
        _logger.LogInformation("Sample service is running");
        
        // This service can be used to demonstrate more complex scenarios
        // where multiple AWS services work together
    }
}