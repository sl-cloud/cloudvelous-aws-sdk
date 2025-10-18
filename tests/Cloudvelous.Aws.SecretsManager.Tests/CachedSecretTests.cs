using Cloudvelous.Aws.SecretsManager;
using FluentAssertions;

namespace Cloudvelous.Aws.SecretsManager.Tests;

public class CachedSecretTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var cachedSecret = new CachedSecret();

        // Assert
        cachedSecret.Value.Should().BeEmpty();
        cachedSecret.VersionId.Should().BeEmpty();
        cachedSecret.Arn.Should().BeEmpty();
        cachedSecret.Name.Should().BeEmpty();
        cachedSecret.CachedAt.Should().Be(default(DateTime));
        cachedSecret.CreatedDate.Should().BeNull();
        cachedSecret.LastModifiedDate.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var cachedSecret = new CachedSecret();
        var value = "test-secret-value";
        var versionId = "test-version-id";
        var arn = "arn:aws:secretsmanager:us-east-1:123456789012:secret:test-secret";
        var name = "test-secret";
        var cachedAt = DateTime.UtcNow;
        var createdDate = DateTime.UtcNow.AddDays(-1);
        var lastModifiedDate = DateTime.UtcNow.AddHours(-1);

        // Act
        cachedSecret.Value = value;
        cachedSecret.VersionId = versionId;
        cachedSecret.Arn = arn;
        cachedSecret.Name = name;
        cachedSecret.CachedAt = cachedAt;
        cachedSecret.CreatedDate = createdDate;
        cachedSecret.LastModifiedDate = lastModifiedDate;

        // Assert
        cachedSecret.Value.Should().Be(value);
        cachedSecret.VersionId.Should().Be(versionId);
        cachedSecret.Arn.Should().Be(arn);
        cachedSecret.Name.Should().Be(name);
        cachedSecret.CachedAt.Should().Be(cachedAt);
        cachedSecret.CreatedDate.Should().Be(createdDate);
        cachedSecret.LastModifiedDate.Should().Be(lastModifiedDate);
    }

    [Fact]
    public void CachedAt_ShouldAcceptCurrentDateTime()
    {
        // Arrange
        var cachedSecret = new CachedSecret();
        var now = DateTime.UtcNow;

        // Act
        cachedSecret.CachedAt = now;

        // Assert
        cachedSecret.CachedAt.Should().Be(now);
    }

    [Fact]
    public void CreatedDate_ShouldAcceptNullableDateTime()
    {
        // Arrange
        var cachedSecret = new CachedSecret();
        var createdDate = DateTime.UtcNow.AddDays(-1);

        // Act
        cachedSecret.CreatedDate = createdDate;

        // Assert
        cachedSecret.CreatedDate.Should().Be(createdDate);
    }

    [Fact]
    public void LastModifiedDate_ShouldAcceptNullableDateTime()
    {
        // Arrange
        var cachedSecret = new CachedSecret();
        var lastModifiedDate = DateTime.UtcNow.AddHours(-1);

        // Act
        cachedSecret.LastModifiedDate = lastModifiedDate;

        // Assert
        cachedSecret.LastModifiedDate.Should().Be(lastModifiedDate);
    }

    [Fact]
    public void CreatedDate_ShouldAcceptNull()
    {
        // Arrange
        var cachedSecret = new CachedSecret();

        // Act
        cachedSecret.CreatedDate = null;

        // Assert
        cachedSecret.CreatedDate.Should().BeNull();
    }

    [Fact]
    public void LastModifiedDate_ShouldAcceptNull()
    {
        // Arrange
        var cachedSecret = new CachedSecret();

        // Act
        cachedSecret.LastModifiedDate = null;

        // Assert
        cachedSecret.LastModifiedDate.Should().BeNull();
    }
}
