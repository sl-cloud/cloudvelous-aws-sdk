# Cloudvelous.Aws.SecretsManager

AWS Secrets Manager client with built-in caching and retry policies.

## Features

- **Secret Management**: Create, read, update, and delete secrets
- **Automatic Caching**: Built-in memory caching to reduce API calls
- **Type Safety**: Strongly-typed secret values with JSON deserialization
- **Retry Policies**: Automatic retry with exponential backoff
- **Dependency Injection**: Easy integration with Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package Cloudvelous.Aws.SecretsManager
```

## Quick Start

```csharp
using Cloudvelous.Aws.SecretsManager;
using Microsoft.Extensions.DependencyInjection;

// Register services
services.AddSecretsManager();

// Use the client
var secretsClient = serviceProvider.GetRequiredService<ISecretsManagerClient>();

// Get a secret
var apiKey = await secretsClient.GetSecretValueAsync<string>("my-api-key");

// Create a secret
var secretArn = await secretsClient.CreateSecretAsync("my-secret", new {
    ApiKey = "secret-api-key",
    DatabaseUrl = "secret-db-url"
});
```

## Configuration

Configure Secrets Manager settings in `appsettings.json`:

```json
{
  "SecretsManager": {
    "DefaultCacheDurationMinutes": 60,
    "EnableCaching": true
  }
}
```

## Documentation

For complete documentation, visit: https://github.com/cloudvelous/cloudvelous-aws-sdk
