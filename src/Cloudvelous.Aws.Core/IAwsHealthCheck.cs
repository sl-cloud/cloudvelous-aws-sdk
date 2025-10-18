namespace Cloudvelous.Aws.Core;

/// <summary>
/// Health check interface for AWS services
/// </summary>
public interface IAwsHealthCheck
{
    /// <summary>
    /// Checks the health of the AWS service
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health check result</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}
