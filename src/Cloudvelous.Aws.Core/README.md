# Cloudvelous.Aws.Core

Core abstractions, options, and retry policies for the Cloudvelous AWS SDK.

## Features

- **Base Abstractions**: Common interfaces and base classes for all AWS services
- **Retry Policies**: Built-in Polly retry policies and circuit breaker patterns
- **Configuration**: Strongly-typed configuration options with validation
- **Logging**: Structured logging support with Microsoft.Extensions.Logging
- **Dependency Injection**: Extension methods for easy service registration

## Installation

```bash
dotnet add package Cloudvelous.Aws.Core
```

## Quick Start

```csharp
using Cloudvelous.Aws.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddAwsCore(options =>
        {
            options.Region = "us-east-1";
            options.RetryPolicy.MaxRetryAttempts = 3;
        });
    })
    .Build();
```

## Configuration

Configure AWS Core settings in `appsettings.json`:

```json
{
  "AWS": {
    "Region": "us-east-1",
    "RetryPolicy": {
      "MaxRetryAttempts": 3,
      "BaseDelay": "00:00:01"
    }
  }
}
```

## Documentation

For complete documentation, visit: https://github.com/sl-cloud/cloudvelous-aws-sdk
