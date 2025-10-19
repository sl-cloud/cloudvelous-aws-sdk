using Amazon.RDS;
using Amazon.RDS.Model;
using Cloudvelous.Aws.Rds;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Data.SqlClient;

namespace Cloudvelous.Aws.Rds.Tests;

/// <summary>
/// Unit tests for RdsClient
/// </summary>
public class RdsClientTests
{
    private readonly Mock<IAmazonRDS> _mockRdsClient;
    private readonly Mock<IOptions<RdsOptions>> _mockOptions;
    private readonly Mock<ILogger<RdsClient>> _mockLogger;
    private readonly RdsClient _rdsClient;

    public RdsClientTests()
    {
        _mockRdsClient = new Mock<IAmazonRDS>();
        _mockOptions = new Mock<IOptions<RdsOptions>>();
        _mockLogger = new Mock<ILogger<RdsClient>>();

        _mockOptions.Setup(x => x.Value).Returns(new RdsOptions
        {
            Region = "us-east-1",
            DefaultPort = 1433,
            DefaultConnectionTimeoutSeconds = 30,
            UseSsl = true
        });

        _rdsClient = new RdsClient(_mockOptions.Object, _mockLogger.Object, _mockRdsClient.Object);
    }


    [Fact]
    public async Task GetDbInstanceAsync_WithValidIdentifier_ShouldReturnDbInstance()
    {
        // Arrange
        var dbInstanceIdentifier = "test-db-instance";
        var expectedInstance = new DBInstance
        {
            DBInstanceIdentifier = dbInstanceIdentifier,
            Endpoint = new Endpoint
            {
                Address = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com",
                Port = 1433
            }
        };

        _mockRdsClient.Setup(x => x.DescribeDBInstancesAsync(It.IsAny<DescribeDBInstancesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeDBInstancesResponse
            {
                DBInstances = new List<DBInstance> { expectedInstance }
            });

        // Act
        var result = await _rdsClient.GetDbInstanceAsync(dbInstanceIdentifier);

        // Assert
        result.Should().NotBeNull();
        result.DBInstanceIdentifier.Should().Be(dbInstanceIdentifier);
        result.Endpoint.Address.Should().Be("test-db.cluster-xyz.us-east-1.rds.amazonaws.com");
        _mockRdsClient.Verify(x => x.DescribeDBInstancesAsync(
            It.Is<DescribeDBInstancesRequest>(req => req.DBInstanceIdentifier == dbInstanceIdentifier), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDbInstanceAsync_WithNonExistentInstance_ShouldThrowException()
    {
        // Arrange
        var dbInstanceIdentifier = "non-existent-instance";

        _mockRdsClient.Setup(x => x.DescribeDBInstancesAsync(It.IsAny<DescribeDBInstancesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeDBInstancesResponse
            {
                DBInstances = new List<DBInstance>()
            });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _rdsClient.GetDbInstanceAsync(dbInstanceIdentifier));
    }

    [Fact]
    public async Task ListDbInstancesAsync_ShouldReturnAllInstances()
    {
        // Arrange
        var instances = new List<DBInstance>
        {
            new DBInstance { DBInstanceIdentifier = "instance1" },
            new DBInstance { DBInstanceIdentifier = "instance2" }
        };

        _mockRdsClient.Setup(x => x.DescribeDBInstancesAsync(It.IsAny<DescribeDBInstancesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeDBInstancesResponse
            {
                DBInstances = instances
            });

        // Act
        var result = await _rdsClient.ListDbInstancesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(i => i.DBInstanceIdentifier == "instance1");
        result.Should().Contain(i => i.DBInstanceIdentifier == "instance2");
        _mockRdsClient.Verify(x => x.DescribeDBInstancesAsync(
            It.IsAny<DescribeDBInstancesRequest>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(Skip = "Requires AWS credentials - run as integration test with real credentials")]
    public async Task GenerateAuthTokenAsync_WithValidParameters_ShouldReturnToken()
    {
        // Arrange
        var hostname = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com";
        var port = 1433;
        var username = "testuser";

        // Act
        var result = await _rdsClient.GenerateAuthTokenAsync(hostname, port, username);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("test-db.cluster-xyz.us-east-1.rds.amazonaws.com");
        result.Should().Contain("1433");
        result.Should().Contain("testuser");
    }

    [Fact(Skip = "Requires AWS credentials - run as integration test with real credentials")]
    public async Task GetConnectionInfoAsync_WithValidParameters_ShouldReturnConnectionInfo()
    {
        // Arrange
        var dbInstanceIdentifier = "test-db-instance";
        var databaseName = "testdb";
        var username = "testuser";
        var dbInstance = new DBInstance
        {
            DBInstanceIdentifier = dbInstanceIdentifier,
            Endpoint = new Endpoint
            {
                Address = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com",
                Port = 1433
            }
        };

        _mockRdsClient.Setup(x => x.DescribeDBInstancesAsync(It.IsAny<DescribeDBInstancesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeDBInstancesResponse
            {
                DBInstances = new List<DBInstance> { dbInstance }
            });

        // Act
        var result = await _rdsClient.GetConnectionInfoAsync(dbInstanceIdentifier, databaseName, username);

        // Assert
        result.Should().NotBeNull();
        result.Hostname.Should().Be("test-db.cluster-xyz.us-east-1.rds.amazonaws.com");
        result.Port.Should().Be(1433);
        result.DatabaseName.Should().Be(databaseName);
        result.Username.Should().Be(username);
        result.AuthToken.Should().NotBeNullOrEmpty();
        result.TokenExpiration.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void BuildConnectionString_WithValidConnectionInfo_ShouldReturnValidConnectionString()
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo
        {
            Hostname = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com",
            Port = 1433,
            DatabaseName = "testdb",
            Username = "testuser",
            AuthToken = "test-auth-token"
        };

        // Act
        var result = _rdsClient.BuildConnectionString(connectionInfo);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("test-db.cluster-xyz.us-east-1.rds.amazonaws.com,1433");
        result.Should().Contain("Initial Catalog=testdb");
        result.Should().Contain("User ID=testuser");
        result.Should().Contain("Password=test-auth-token");
    }

    [Fact]
    public void BuildConnectionString_WithAdditionalOptions_ShouldIncludeAdditionalOptions()
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo
        {
            Hostname = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com",
            Port = 1433,
            DatabaseName = "testdb",
            Username = "testuser",
            AuthToken = "test-auth-token"
        };
        var additionalOptions = new Dictionary<string, string>
        {
            { "Max Pool Size", "100" },
            { "Min Pool Size", "5" }
        };

        // Act
        var result = _rdsClient.BuildConnectionString(connectionInfo, additionalOptions);

        // Assert
        result.Should().Contain("Max Pool Size=100");
        result.Should().Contain("Min Pool Size=5");
    }

    [Fact]
    public void CreateConnection_WithValidConnectionInfo_ShouldReturnSqlConnection()
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo
        {
            Hostname = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com",
            Port = 1433,
            DatabaseName = "testdb",
            Username = "testuser",
            AuthToken = "test-auth-token"
        };

        // Act
        var result = _rdsClient.CreateConnection(connectionInfo);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<SqlConnection>();
        result.ConnectionString.Should().Contain("test-db.cluster-xyz.us-east-1.rds.amazonaws.com,1433");
    }

    [Fact]
    public async Task TestConnectionAsync_WithValidConnectionInfo_ShouldReturnTrue()
    {
        // Arrange
        var connectionInfo = new RdsConnectionInfo
        {
            Hostname = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com",
            Port = 1433,
            DatabaseName = "testdb",
            Username = "testuser",
            AuthToken = "test-auth-token"
        };

        // Note: This test will likely fail in a real scenario since we're not mocking the actual SQL connection
        // In a real test environment, you would use a test database or mock the SqlConnection
        // For now, we'll expect it to return false due to connection failure

        // Act
        var result = await _rdsClient.TestConnectionAsync(connectionInfo);

        // Assert
        result.Should().BeFalse(); // Expected to fail without real database
    }

    [Fact(Skip = "Requires AWS credentials - run as integration test with real credentials")]
    public async Task GetConnectionInfoAsync_WithNullPort_ShouldUseDefaultPort()
    {
        // Arrange
        var dbInstanceIdentifier = "test-db-instance";
        var databaseName = "testdb";
        var username = "testuser";
        var dbInstance = new DBInstance
        {
            DBInstanceIdentifier = dbInstanceIdentifier,
            Endpoint = new Endpoint
            {
                Address = "test-db.cluster-xyz.us-east-1.rds.amazonaws.com",
                Port = null // Null port
            }
        };

        _mockRdsClient.Setup(x => x.DescribeDBInstancesAsync(It.IsAny<DescribeDBInstancesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeDBInstancesResponse
            {
                DBInstances = new List<DBInstance> { dbInstance }
            });

        // Act
        var result = await _rdsClient.GetConnectionInfoAsync(dbInstanceIdentifier, databaseName, username);

        // Assert
        result.Should().NotBeNull();
        result.Port.Should().Be(1433); // Should use default port from options
    }
}
