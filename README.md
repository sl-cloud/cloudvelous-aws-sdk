# Cloudvelous AWS SDK

A modular .NET 8 AWS SDK with project-per-service design, providing clean abstractions, built-in retry policies, and comprehensive logging support.

## Features

- **Modular Design**: Each AWS service is a separate NuGet package
- **Clean Abstractions**: Simple, intuitive interfaces for all AWS services
- **Built-in Resilience**: Polly retry policies and circuit breaker patterns
- **Comprehensive Logging**: Structured logging with Microsoft.Extensions.Logging
- **Dependency Injection**: Full DI support with extension methods
- **Health Checks**: Built-in health check implementations
- **Strong Typing**: Type-safe operations with JSON serialization support
- **IAM Authentication**: Support for RDS IAM authentication tokens

## Services

| Service | Package | Description |
|---------|---------|-------------|
| **Core** | `Cloudvelous.Aws.Core` | Base abstractions, options, and retry policies |
| **SQS** | `Cloudvelous.Aws.Sqs` | Simple Queue Service with batch operations |
| **RDS** | `Cloudvelous.Aws.Rds` | Relational Database Service with IAM auth |
| **Secrets Manager** | `Cloudvelous.Aws.SecretsManager` | Secrets management with caching |
| **Lambda** | `Cloudvelous.Aws.Lambda` | Serverless function invocation |
| **OpenSearch** | `Cloudvelous.Aws.OpenSearch` | Search and analytics service |
| **DynamoDB** | `Cloudvelous.Aws.DynamoDB` | NoSQL database with typed operations |

## Quick Start

### Installation

> 📖 **For detailed installation instructions, see [INSTALLATION.md](INSTALLATION.md)**

These packages are publicly available on **NuGet.org**. Install via the .NET CLI:

```bash
# Core package (required)
dotnet add package Cloudvelous.Aws.Core

# Service packages (install as needed)
dotnet add package Cloudvelous.Aws.SecretsManager
dotnet add package Cloudvelous.Aws.Sqs
dotnet add package Cloudvelous.Aws.Rds
dotnet add package Cloudvelous.Aws.Lambda
dotnet add package Cloudvelous.Aws.DynamoDB
dotnet add package Cloudvelous.Aws.OpenSearch
```

Or search for "Cloudvelous.Aws" in the Visual Studio NuGet Package Manager.

### Basic Usage

```csharp
using Cloudvelous.Aws.Core;
using Cloudvelous.Aws.Sqs;
using Cloudvelous.Aws.SecretsManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Add AWS Core services
        services.AddAwsCore(options =>
        {
            options.Region = "us-east-1";
            options.RetryPolicy.MaxRetryAttempts = 3;
        });

        // Add specific AWS services
        services.AddSqs();
        services.AddSecretsManager();
        services.AddRds();
    })
    .Build();

// Use the services
var sqsClient = host.Services.GetRequiredService<ISqsClient>();
var secretsClient = host.Services.GetRequiredService<ISecretsManagerClient>();

// Send a message to SQS
var queueUrl = await sqsClient.CreateQueueAsync("my-queue");
await sqsClient.SendMessageAsync(queueUrl, new { Message = "Hello World!" });

// Get a secret
var apiKey = await secretsClient.GetSecretValueAsync<string>("my-api-key");
```

## Configuration

### appsettings.json

```json
{
  "AWS": {
    "Region": "us-east-1"
  },
  "Sqs": {
    "DefaultVisibilityTimeoutSeconds": 30,
    "MaxReceiveMessages": 10
  },
  "SecretsManager": {
    "DefaultCacheDurationMinutes": 60,
    "EnableCaching": true
  },
  "Rds": {
    "DefaultPort": 1433,
    "UseSsl": true
  }
}
```

### Environment Variables

```bash
export AWS_REGION=us-east-1
export AWS_ACCESS_KEY_ID=your-access-key
export AWS_SECRET_ACCESS_KEY=your-secret-key
```

## Examples

### SQS Operations

```csharp
var sqsClient = serviceProvider.GetRequiredService<ISqsClient>();

// Create a queue
var queueUrl = await sqsClient.CreateQueueAsync("my-queue");

// Send a message
var messageId = await sqsClient.SendMessageAsync(queueUrl, new { 
    Text = "Hello World!", 
    Timestamp = DateTime.UtcNow 
});

// Receive messages
var messages = await sqsClient.ReceiveMessagesAsync<MyMessageType>(queueUrl);

// Delete messages
foreach (var message in messages)
{
    await sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
}
```

### Secrets Manager with Caching

```csharp
var secretsClient = serviceProvider.GetRequiredService<ISecretsManagerClient>();

// Create a secret
var secretArn = await secretsClient.CreateSecretAsync("my-secret", new {
    ApiKey = "secret-api-key",
    DatabaseUrl = "secret-db-url"
});

// Get secret (cached automatically)
var secret = await secretsClient.GetSecretValueAsync<MySecretType>("my-secret");

// Update secret (cache invalidated automatically)
await secretsClient.UpdateSecretAsync("my-secret", new {
    ApiKey = "new-api-key",
    DatabaseUrl = "new-db-url"
});
```

### RDS with IAM Authentication

```csharp
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

## Testing

The SDK includes comprehensive test projects using xUnit and Testcontainers with LocalStack for integration testing.

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Cloudvelous.Aws.Sqs.Tests/
```

## Building from Source

```bash
# Clone the repository
git clone https://github.com/sl-cloud/cloudvelous-aws-sdk.git
cd cloudvelous-aws-sdk

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run sample application
dotnet run --project samples/Cloudvelous.Aws.Samples.Console/
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Documentation

- 📦 [Installation Guide](INSTALLATION.md) - Detailed setup instructions
- 📖 [Package Documentation](src/) - Service-specific READMEs
- 💻 [Sample Application](samples/Cloudvelous.Aws.Samples.Console/) - Working examples

## Support

- 🐛 [Issue Tracker](https://github.com/sl-cloud/cloudvelous-aws-sdk/issues)
- 💬 [Discussions](https://github.com/sl-cloud/cloudvelous-aws-sdk/discussions)

## Roadmap

- [ ] Additional AWS services (S3, SNS, EventBridge, etc.)
- [ ] Performance optimizations
- [ ] Advanced caching strategies
- [ ] Metrics and observability
- [ ] Multi-region support
- [ ] Custom retry policies per service
