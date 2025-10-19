# Cloudvelous.Aws.Rds

AWS Relational Database Service (RDS) client with IAM authentication support.

## Features

- **IAM Authentication**: Generate RDS IAM authentication tokens
- **Connection Management**: Build connection strings and create database connections
- **Type Safety**: Strongly-typed connection information
- **Retry Policies**: Automatic retry with exponential backoff
- **Dependency Injection**: Easy integration with Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package Cloudvelous.Aws.Rds
```

## Quick Start

```csharp
using Cloudvelous.Aws.Rds;
using Microsoft.Extensions.DependencyInjection;

// Register services
services.AddRds();

// Use the client
var rdsClient = serviceProvider.GetRequiredService<IRdsClient>();

// Get connection info with IAM auth token
var connectionInfo = await rdsClient.GetConnectionInfoAsync(
    "my-db-instance", 
    "my-database", 
    "my-username"
);

// Build connection string
var connectionString = rdsClient.BuildConnectionString(connectionInfo);

// Create SQL connection
using var connection = rdsClient.CreateConnection(connectionInfo);
await connection.OpenAsync();
```

## Configuration

Configure RDS settings in `appsettings.json`:

```json
{
  "Rds": {
    "DefaultPort": 1433,
    "UseSsl": true
  }
}
```

## Documentation

For complete documentation, visit: https://github.com/cloudvelous/cloudvelous-aws-sdk
