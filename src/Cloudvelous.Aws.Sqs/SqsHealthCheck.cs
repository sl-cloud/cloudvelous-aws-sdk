using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Sqs;
using Microsoft.Extensions.Logging;

namespace Cloudvelous.Aws.Sqs.Extensions;

/// <summary>
/// SQS health check implementation
/// </summary>
public class SqsHealthCheck : AwsHealthCheckBase<ISqsClient>
{
    /// <summary>
    /// Initializes a new instance of the SQS health check
    /// </summary>
    /// <param name="client">SQS client instance</param>
    /// <param name="logger">Logger instance</param>
    public SqsHealthCheck(ISqsClient client, ILogger<SqsHealthCheck> logger) 
        : base(client, logger)
    {
    }

    /// <inheritdoc />
    protected override async Task<bool> PerformHealthCheckAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Try to list queues as a health check
            // This is a lightweight operation that doesn't require a specific queue
            await Client.GetQueueUrlAsync("test-health-check-queue", cancellationToken);
            return true;
        }
        catch (Amazon.SQS.AmazonSQSException ex) when (ex.ErrorCode == "AWS.SimpleQueueService.NonExistentQueue")
        {
            // Queue doesn't exist, but service is responding
            return true;
        }
        catch
        {
            return false;
        }
    }
}
