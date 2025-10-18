using Cloudvelous.Aws.Rds;
using FluentAssertions;

namespace Cloudvelous.Aws.Rds.Tests;

public class RdsConnectionInfoTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var connectionInfo = new RdsConnectionInfo();

        // Assert
        connectionInfo.Hostname.Should().BeEmpty();
        connectionInfo.Port.Should().Be(0);
        connectionInfo.DatabaseName.Should().BeEmpty();
        connectionInfo.Username.Should().BeEmpty();
        connectionInfo.AuthToken.Should().BeEmpty();
        connectionInfo.TokenExpiration.Should().Be(default(DateTime));
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo();
        var hostname = "test-db.amazonaws.com";
        var port = 1433;
        var databaseName = "testdb";
        var username = "testuser";
        var authToken = "test-auth-token";
        var tokenExpiration = DateTime.UtcNow.AddHours(1);

        // Act
        connectionInfo.Hostname = hostname;
        connectionInfo.Port = port;
        connectionInfo.DatabaseName = databaseName;
        connectionInfo.Username = username;
        connectionInfo.AuthToken = authToken;
        connectionInfo.TokenExpiration = tokenExpiration;

        // Assert
        connectionInfo.Hostname.Should().Be(hostname);
        connectionInfo.Port.Should().Be(port);
        connectionInfo.DatabaseName.Should().Be(databaseName);
        connectionInfo.Username.Should().Be(username);
        connectionInfo.AuthToken.Should().Be(authToken);
        connectionInfo.TokenExpiration.Should().Be(tokenExpiration);
    }

    [Theory]
    [InlineData(1433)]
    [InlineData(3306)]
    [InlineData(5432)]
    [InlineData(1521)]
    public void Port_ShouldAcceptValidDatabasePorts(int port)
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo();

        // Act
        connectionInfo.Port = port;

        // Assert
        connectionInfo.Port.Should().Be(port);
    }

    [Fact]
    public void TokenExpiration_ShouldAcceptFutureDateTime()
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo();
        var futureTime = DateTime.UtcNow.AddHours(1);

        // Act
        connectionInfo.TokenExpiration = futureTime;

        // Assert
        connectionInfo.TokenExpiration.Should().Be(futureTime);
    }

    [Fact]
    public void TokenExpiration_ShouldAcceptPastDateTime()
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo();
        var pastTime = DateTime.UtcNow.AddHours(-1);

        // Act
        connectionInfo.TokenExpiration = pastTime;

        // Assert
        connectionInfo.TokenExpiration.Should().Be(pastTime);
    }
}
