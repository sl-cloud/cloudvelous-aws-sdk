using Amazon.SecretsManager.Model;

namespace Cloudvelous.Aws.SecretsManager;

/// <summary>
/// Secrets Manager client interface
/// </summary>
public interface ISecretsManagerClient
{
    /// <summary>
    /// Gets a secret value by name
    /// </summary>
    /// <param name="secretName">Secret name</param>
    /// <param name="versionId">Optional version ID</param>
    /// <param name="useCache">Whether to use cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secret value</returns>
    Task<string> GetSecretValueAsync(string secretName, string? versionId = null, bool? useCache = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a secret value as a strongly-typed object
    /// </summary>
    /// <typeparam name="T">Secret value type</typeparam>
    /// <param name="secretName">Secret name</param>
    /// <param name="versionId">Optional version ID</param>
    /// <param name="useCache">Whether to use cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized secret value</returns>
    Task<T> GetSecretValueAsync<T>(string secretName, string? versionId = null, bool? useCache = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cached secret information
    /// </summary>
    /// <param name="secretName">Secret name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached secret information</returns>
    Task<CachedSecret?> GetCachedSecretAsync(string secretName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a secret
    /// </summary>
    /// <param name="secretName">Secret name</param>
    /// <param name="secretValue">Secret value</param>
    /// <param name="description">Optional description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secret ARN</returns>
    Task<string> CreateSecretAsync(string secretName, string secretValue, string? description = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a secret with a strongly-typed object
    /// </summary>
    /// <typeparam name="T">Secret value type</typeparam>
    /// <param name="secretName">Secret name</param>
    /// <param name="secretValue">Secret value</param>
    /// <param name="description">Optional description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secret ARN</returns>
    Task<string> CreateSecretAsync<T>(string secretName, T secretValue, string? description = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a secret value
    /// </summary>
    /// <param name="secretName">Secret name</param>
    /// <param name="secretValue">New secret value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secret ARN</returns>
    Task<string> UpdateSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a secret value with a strongly-typed object
    /// </summary>
    /// <typeparam name="T">Secret value type</typeparam>
    /// <param name="secretName">Secret name</param>
    /// <param name="secretValue">New secret value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secret ARN</returns>
    Task<string> UpdateSecretAsync<T>(string secretName, T secretValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a secret
    /// </summary>
    /// <param name="secretName">Secret name</param>
    /// <param name="forceDeleteWithoutRecovery">Whether to force delete without recovery</param>
    /// <param name="recoveryWindowInDays">Recovery window in days</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secret ARN</returns>
    Task<string> DeleteSecretAsync(string secretName, bool forceDeleteWithoutRecovery = false, int recoveryWindowInDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all secrets
    /// </summary>
    /// <param name="maxResults">Maximum number of results</param>
    /// <param name="nextToken">Next token for pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of secret list entries</returns>
    Task<List<SecretListEntry>> ListSecretsAsync(int maxResults = 100, string? nextToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Describes a secret
    /// </summary>
    /// <param name="secretName">Secret name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secret description</returns>
    Task<DescribeSecretResponse> DescribeSecretAsync(string secretName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cached secret
    /// </summary>
    /// <param name="secretName">Secret name</param>
    void InvalidateCache(string secretName);

    /// <summary>
    /// Clears all cached secrets
    /// </summary>
    void ClearCache();
}
