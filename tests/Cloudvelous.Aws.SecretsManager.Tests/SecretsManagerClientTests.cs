using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Cloudvelous.Aws.SecretsManager;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;

namespace Cloudvelous.Aws.SecretsManager.Tests;

/// <summary>
/// Unit tests for SecretsManagerClient
/// </summary>
public class SecretsManagerClientTests
{
    private readonly Mock<IAmazonSecretsManager> _mockSecretsManagerClient;
    private readonly Mock<IOptions<SecretsManagerOptions>> _mockOptions;
    private readonly Mock<ILogger<SecretsManagerClient>> _mockLogger;
    private readonly IMemoryCache _cache;
    private readonly SecretsManagerClient _secretsManagerClient;

    public SecretsManagerClientTests()
    {
        _mockSecretsManagerClient = new Mock<IAmazonSecretsManager>();
        _mockOptions = new Mock<IOptions<SecretsManagerOptions>>();
        _mockLogger = new Mock<ILogger<SecretsManagerClient>>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _mockOptions.Setup(x => x.Value).Returns(new SecretsManagerOptions
        {
            Region = "us-east-1",
            DefaultCacheDurationMinutes = 60,
            EnableCaching = true
        });

        _secretsManagerClient = new SecretsManagerClient(_mockOptions.Object, _mockLogger.Object, _cache, _mockSecretsManagerClient.Object);
    }


    [Fact]
    public async Task GetSecretValueAsync_WithValidSecretName_ShouldReturnSecretValue()
    {
        // Arrange
        var secretName = "test-secret";
        var expectedValue = "secret-value";
        var cacheKey = $"{secretName}:";

        _mockSecretsManagerClient.Setup(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSecretValueResponse
            {
                SecretString = expectedValue,
                ARN = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret",
                Name = secretName
            });

        // Act
        var result = await _secretsManagerClient.GetSecretValueAsync(secretName);

        // Assert
        result.Should().Be(expectedValue);
        _mockSecretsManagerClient.Verify(x => x.GetSecretValueAsync(
            It.Is<GetSecretValueRequest>(req => req.SecretId == secretName), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSecretValueAsync_WithCachedValue_ShouldReturnCachedValue()
    {
        // Arrange
        var secretName = "test-secret";
        var cachedValue = "cached-secret-value";
        var cacheKey = $"{secretName}:";

        // Pre-populate the cache
        _cache.Set(cacheKey, cachedValue, TimeSpan.FromMinutes(60));

        // Act
        var result = await _secretsManagerClient.GetSecretValueAsync(secretName);

        // Assert
        result.Should().Be(cachedValue);
        _mockSecretsManagerClient.Verify(x => x.GetSecretValueAsync(
            It.IsAny<GetSecretValueRequest>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetSecretValueAsync_WithVersionId_ShouldIncludeVersionId()
    {
        // Arrange
        var secretName = "test-secret";
        var versionId = "version-123";
        var expectedValue = "secret-value";
        var cacheKey = $"{secretName}:{versionId}";

        _mockSecretsManagerClient.Setup(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSecretValueResponse
            {
                SecretString = expectedValue,
                VersionId = versionId
            });

        // Act
        var result = await _secretsManagerClient.GetSecretValueAsync(secretName, versionId);

        // Assert
        result.Should().Be(expectedValue);
        _mockSecretsManagerClient.Verify(x => x.GetSecretValueAsync(
            It.Is<GetSecretValueRequest>(req => 
                req.SecretId == secretName && 
                req.VersionId == versionId), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSecretValueAsync_WithBinarySecret_ShouldReturnBinaryAsString()
    {
        // Arrange
        var secretName = "test-binary-secret";
        var binaryData = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("binary-secret-value"));


        _mockSecretsManagerClient.Setup(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSecretValueResponse
            {
                SecretBinary = binaryData
            });

        // Act
        var result = await _secretsManagerClient.GetSecretValueAsync(secretName);

        // Assert
        result.Should().Be("binary-secret-value");
    }

    [Fact]
    public async Task GetSecretValueAsync_WithNoStringOrBinary_ShouldThrowException()
    {
        // Arrange
        var secretName = "test-secret";


        _mockSecretsManagerClient.Setup(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSecretValueResponse
            {
                SecretString = null,
                SecretBinary = null
            });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _secretsManagerClient.GetSecretValueAsync(secretName));
    }

    [Fact]
    public async Task GetSecretValueAsyncT_WithValidSecret_ShouldReturnDeserializedObject()
    {
        // Arrange
        var secretName = "test-secret";
        var secretObject = new { Name = "test", Value = 123 };
        var secretJson = JsonSerializer.Serialize(secretObject);


        _mockSecretsManagerClient.Setup(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSecretValueResponse
            {
                SecretString = secretJson
            });

        // Act
        var result = await _secretsManagerClient.GetSecretValueAsync<object>(secretName);

        // Assert
        result.Should().NotBeNull();
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(result.ToString()!);
        deserialized.Should().ContainKey("Name");
        deserialized.Should().ContainKey("Value");
    }

    [Fact]
    public async Task GetCachedSecretAsync_WithCachedSecret_ShouldReturnCachedSecret()
    {
        // Arrange
        var secretName = "test-secret";
        var cachedValue = "cached-value";
        var cacheKey = $"{secretName}:";

        // Pre-populate the cache
        _cache.Set(cacheKey, cachedValue, TimeSpan.FromMinutes(60));

        // Act
        var result = await _secretsManagerClient.GetCachedSecretAsync(secretName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(secretName);
        result.Value.Should().Be(cachedValue);
    }

    [Fact]
    public async Task GetCachedSecretAsync_WithNoCachedSecret_ShouldReturnNull()
    {
        // Arrange
        var secretName = "test-secret";
        var cacheKey = $"{secretName}:";

        // Act
        var result = await _secretsManagerClient.GetCachedSecretAsync(secretName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateSecretAsync_WithValidParameters_ShouldReturnArn()
    {
        // Arrange
        var secretName = "test-secret";
        var secretValue = "secret-value";
        var description = "Test secret";
        var expectedArn = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret";

        _mockSecretsManagerClient.Setup(x => x.CreateSecretAsync(It.IsAny<CreateSecretRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateSecretResponse
            {
                ARN = expectedArn
            });

        // Act
        var result = await _secretsManagerClient.CreateSecretAsync(secretName, secretValue, description);

        // Assert
        result.Should().Be(expectedArn);
        _mockSecretsManagerClient.Verify(x => x.CreateSecretAsync(
            It.Is<CreateSecretRequest>(req => 
                req.Name == secretName && 
                req.SecretString == secretValue &&
                req.Description == description), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSecretAsyncT_WithValidObject_ShouldSerializeAndCreateSecret()
    {
        // Arrange
        var secretName = "test-secret";
        var secretObject = new { Name = "test", Value = 123 };
        var expectedArn = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret";

        _mockSecretsManagerClient.Setup(x => x.CreateSecretAsync(It.IsAny<CreateSecretRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateSecretResponse
            {
                ARN = expectedArn
            });

        // Act
        var result = await _secretsManagerClient.CreateSecretAsync(secretName, secretObject);

        // Assert
        result.Should().Be(expectedArn);
        _mockSecretsManagerClient.Verify(x => x.CreateSecretAsync(
            It.Is<CreateSecretRequest>(req => 
                req.Name == secretName && 
                req.SecretString.Contains("test")), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSecretAsync_WithValidParameters_ShouldReturnArn()
    {
        // Arrange
        var secretName = "test-secret";
        var secretValue = "updated-secret-value";
        var expectedArn = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret";

        _mockSecretsManagerClient.Setup(x => x.UpdateSecretAsync(It.IsAny<UpdateSecretRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UpdateSecretResponse
            {
                ARN = expectedArn
            });

        // Act
        var result = await _secretsManagerClient.UpdateSecretAsync(secretName, secretValue);

        // Assert
        result.Should().Be(expectedArn);
        _mockSecretsManagerClient.Verify(x => x.UpdateSecretAsync(
            It.Is<UpdateSecretRequest>(req => 
                req.SecretId == secretName && 
                req.SecretString == secretValue), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSecretAsyncT_WithValidObject_ShouldSerializeAndUpdateSecret()
    {
        // Arrange
        var secretName = "test-secret";
        var secretObject = new { Name = "updated", Value = 456 };
        var expectedArn = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret";

        _mockSecretsManagerClient.Setup(x => x.UpdateSecretAsync(It.IsAny<UpdateSecretRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UpdateSecretResponse
            {
                ARN = expectedArn
            });

        // Act
        var result = await _secretsManagerClient.UpdateSecretAsync(secretName, secretObject);

        // Assert
        result.Should().Be(expectedArn);
        _mockSecretsManagerClient.Verify(x => x.UpdateSecretAsync(
            It.Is<UpdateSecretRequest>(req => 
                req.SecretId == secretName && 
                req.SecretString.Contains("updated")), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSecretAsync_WithValidParameters_ShouldReturnArn()
    {
        // Arrange
        var secretName = "test-secret";
        var forceDelete = true;
        var recoveryWindow = 7;
        var expectedArn = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret";

        _mockSecretsManagerClient.Setup(x => x.DeleteSecretAsync(It.IsAny<DeleteSecretRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteSecretResponse
            {
                ARN = expectedArn
            });

        // Act
        var result = await _secretsManagerClient.DeleteSecretAsync(secretName, forceDelete, recoveryWindow);

        // Assert
        result.Should().Be(expectedArn);
        _mockSecretsManagerClient.Verify(x => x.DeleteSecretAsync(
            It.Is<DeleteSecretRequest>(req => 
                req.SecretId == secretName && 
                req.ForceDeleteWithoutRecovery == forceDelete &&
                req.RecoveryWindowInDays == recoveryWindow), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListSecretsAsync_WithValidParameters_ShouldReturnSecretList()
    {
        // Arrange
        var maxResults = 50;
        var nextToken = "next-token";
        var secrets = new List<SecretListEntry>
        {
            new SecretListEntry { Name = "secret1", ARN = "arn1" },
            new SecretListEntry { Name = "secret2", ARN = "arn2" }
        };

        _mockSecretsManagerClient.Setup(x => x.ListSecretsAsync(It.IsAny<ListSecretsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListSecretsResponse
            {
                SecretList = secrets
            });

        // Act
        var result = await _secretsManagerClient.ListSecretsAsync(maxResults, nextToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.Name == "secret1");
        result.Should().Contain(s => s.Name == "secret2");
        _mockSecretsManagerClient.Verify(x => x.ListSecretsAsync(
            It.Is<ListSecretsRequest>(req => 
                req.MaxResults == maxResults && 
                req.NextToken == nextToken), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DescribeSecretAsync_WithValidSecretName_ShouldReturnDescription()
    {
        // Arrange
        var secretName = "test-secret";
        var expectedResponse = new DescribeSecretResponse
        {
            Name = secretName,
            ARN = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret",
            Description = "Test secret description"
        };

        _mockSecretsManagerClient.Setup(x => x.DescribeSecretAsync(It.IsAny<DescribeSecretRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _secretsManagerClient.DescribeSecretAsync(secretName);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(secretName);
        result.Description.Should().Be("Test secret description");
        _mockSecretsManagerClient.Verify(x => x.DescribeSecretAsync(
            It.Is<DescribeSecretRequest>(req => req.SecretId == secretName), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void InvalidateCache_WithValidSecretName_ShouldRemoveFromCache()
    {
        // Arrange
        var secretName = "test-secret";
        var cacheKey = $"{secretName}:";
        
        // Pre-populate the cache
        _cache.Set(cacheKey, "cached-value", TimeSpan.FromMinutes(60));
        
        // Verify it's in cache
        _cache.TryGetValue(cacheKey, out _).Should().BeTrue();

        // Act
        _secretsManagerClient.InvalidateCache(secretName);

        // Assert
        _cache.TryGetValue(cacheKey, out _).Should().BeFalse();
    }

    [Fact]
    public void ClearCache_ShouldLogDebugMessage()
    {
        // Act
        _secretsManagerClient.ClearCache();

        // Assert
        // Since IMemoryCache doesn't have a direct Clear method, we just verify the method doesn't throw
        // In a real implementation, you might use a custom cache wrapper or dispose/recreate the cache
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Clearing all Secrets Manager cache entries")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
