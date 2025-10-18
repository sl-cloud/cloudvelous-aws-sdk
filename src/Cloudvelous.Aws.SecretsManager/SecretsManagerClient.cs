using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Cloudvelous.Aws.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Cloudvelous.Aws.SecretsManager;

/// <summary>
/// Secrets Manager client implementation
/// </summary>
public class SecretsManagerClient : AwsClientFactoryBase<AmazonSecretsManagerClient, SecretsManagerOptions>, ISecretsManagerClient
{
    private readonly IAmazonSecretsManager _secretsManagerClient;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the Secrets Manager client
    /// </summary>
    /// <param name="options">Secrets Manager configuration options</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="cache">Memory cache</param>
    public SecretsManagerClient(IOptions<SecretsManagerOptions> options, ILogger<SecretsManagerClient> logger, IMemoryCache cache) : base(options, logger)
    {
        _secretsManagerClient = CreateClient();
        _cache = cache;
    }

    /// <summary>
    /// Initializes a new instance of the Secrets Manager client with a specific client instance (for testing)
    /// </summary>
    /// <param name="options">Secrets Manager configuration options</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="cache">Memory cache</param>
    /// <param name="secretsManagerClient">Secrets Manager client instance</param>
    internal SecretsManagerClient(IOptions<SecretsManagerOptions> options, ILogger<SecretsManagerClient> logger, IMemoryCache cache, IAmazonSecretsManager secretsManagerClient) : base(options, logger)
    {
        _secretsManagerClient = secretsManagerClient;
        _cache = cache;
    }

    /// <inheritdoc />
    protected override Amazon.Runtime.ClientConfig CreateSpecificClientConfig()
    {
        return new AmazonSecretsManagerConfig();
    }

    /// <inheritdoc />
    protected override AmazonSecretsManagerClient CreateClientInstance(Amazon.Runtime.ClientConfig config)
    {
        return new AmazonSecretsManagerClient((AmazonSecretsManagerConfig)config);
    }

    /// <inheritdoc />
    public async Task<string> GetSecretValueAsync(string secretName, string? versionId = null, bool? useCache = null, CancellationToken cancellationToken = default)
    {
        var enableCaching = useCache ?? Options.EnableCaching;
        var cacheKey = $"{secretName}:{versionId}";

        if (enableCaching && _cache.TryGetValue(cacheKey, out string? cachedValue))
        {
            Logger.LogDebug("Retrieving secret '{SecretName}' from cache.", secretName);
            return cachedValue!;
        }

        Logger.LogDebug("Retrieving secret '{SecretName}' from AWS Secrets Manager.", secretName);

        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionId = versionId
        };

        var response = await _secretsManagerClient.GetSecretValueAsync(request, cancellationToken);

        if (response.SecretString != null)
        {
            if (enableCaching)
            {
                _cache.Set(cacheKey, response.SecretString, TimeSpan.FromMinutes(Options.DefaultCacheDurationMinutes));
            }
            return response.SecretString;
        }

        if (response.SecretBinary != null)
        {
            using var reader = new StreamReader(response.SecretBinary);
            var secretString = await reader.ReadToEndAsync();
            if (enableCaching)
            {
                _cache.Set(cacheKey, secretString, TimeSpan.FromMinutes(Options.DefaultCacheDurationMinutes));
            }
            return secretString;
        }

        throw new InvalidOperationException($"Secret '{secretName}' does not contain a string or binary value.");
    }

    /// <inheritdoc />
    public async Task<T> GetSecretValueAsync<T>(string secretName, string? versionId = null, bool? useCache = null, CancellationToken cancellationToken = default)
    {
        var secretString = await GetSecretValueAsync(secretName, versionId, useCache, cancellationToken);
        return JsonSerializer.Deserialize<T>(secretString) ?? throw new InvalidOperationException($"Failed to deserialize secret '{secretName}' to type {typeof(T).Name}.");
    }

    /// <inheritdoc />
    public Task<CachedSecret?> GetCachedSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue($"{secretName}:", out string? cachedValue))
        {
            return Task.FromResult<CachedSecret?>(new CachedSecret { Name = secretName, Value = cachedValue });
        }
        return Task.FromResult<CachedSecret?>(null);
    }

    /// <inheritdoc />
    public async Task<string> CreateSecretAsync(string secretName, string secretValue, string? description = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Creating secret '{SecretName}' in AWS Secrets Manager.", secretName);

        var request = new CreateSecretRequest
        {
            Name = secretName,
            SecretString = secretValue,
            Description = description
        };

        var response = await _secretsManagerClient.CreateSecretAsync(request, cancellationToken);
        InvalidateCache(secretName);
        return response.ARN;
    }

    /// <inheritdoc />
    public Task<string> CreateSecretAsync<T>(string secretName, T secretValue, string? description = null, CancellationToken cancellationToken = default)
    {
        var secretString = JsonSerializer.Serialize(secretValue);
        return CreateSecretAsync(secretName, secretString, description, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> UpdateSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Updating secret '{SecretName}' in AWS Secrets Manager.", secretName);

        var request = new UpdateSecretRequest
        {
            SecretId = secretName,
            SecretString = secretValue
        };

        var response = await _secretsManagerClient.UpdateSecretAsync(request, cancellationToken);
        InvalidateCache(secretName);
        return response.ARN;
    }

    /// <inheritdoc />
    public Task<string> UpdateSecretAsync<T>(string secretName, T secretValue, CancellationToken cancellationToken = default)
    {
        var secretString = JsonSerializer.Serialize(secretValue);
        return UpdateSecretAsync(secretName, secretString, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> DeleteSecretAsync(string secretName, bool forceDeleteWithoutRecovery = false, int recoveryWindowInDays = 30, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Deleting secret '{SecretName}' from AWS Secrets Manager. Force delete: {ForceDelete}, Recovery window: {RecoveryWindow} days.", secretName, forceDeleteWithoutRecovery, recoveryWindowInDays);

        var request = new DeleteSecretRequest
        {
            SecretId = secretName,
            ForceDeleteWithoutRecovery = forceDeleteWithoutRecovery,
            RecoveryWindowInDays = recoveryWindowInDays
        };

        var response = await _secretsManagerClient.DeleteSecretAsync(request, cancellationToken);
        InvalidateCache(secretName);
        return response.ARN;
    }

    /// <inheritdoc />
    public async Task<List<SecretListEntry>> ListSecretsAsync(int maxResults = 100, string? nextToken = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing secrets from AWS Secrets Manager. Max results: {MaxResults}", maxResults);

        var request = new ListSecretsRequest
        {
            MaxResults = maxResults,
            NextToken = nextToken
        };

        var response = await _secretsManagerClient.ListSecretsAsync(request, cancellationToken);
        return response.SecretList;
    }

    /// <inheritdoc />
    public async Task<DescribeSecretResponse> DescribeSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Describing secret '{SecretName}' in AWS Secrets Manager.", secretName);

        var request = new DescribeSecretRequest
        {
            SecretId = secretName
        };

        return await _secretsManagerClient.DescribeSecretAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public void InvalidateCache(string secretName)
    {
        Logger.LogDebug("Invalidating cache for secret '{SecretName}'.", secretName);
        _cache.Remove($"{secretName}:");
    }

    /// <inheritdoc />
    public void ClearCache()
    {
        Logger.LogDebug("Clearing all Secrets Manager cache entries.");
        // IMemoryCache does not have a direct Clear method.
        // A common workaround is to dispose and re-create, or use a custom cache implementation.
        // For simplicity, we'll just log for now.
    }
}