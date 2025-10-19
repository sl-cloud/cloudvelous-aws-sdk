# Cloudvelous.Aws.DynamoDB

AWS DynamoDB client with typed operations and retry policies.

## Features

- **Document Operations**: Create, read, update, and delete documents
- **Type Safety**: Strongly-typed document operations with JSON serialization
- **Retry Policies**: Automatic retry with exponential backoff
- **Dependency Injection**: Easy integration with Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package Cloudvelous.Aws.DynamoDB
```

## Quick Start

```csharp
using Cloudvelous.Aws.DynamoDB;
using Microsoft.Extensions.DependencyInjection;

// Register services
services.AddDynamoDB();

// Use the client
var dynamoClient = serviceProvider.GetRequiredService<IDynamoDBClient>();

// Put an item
await dynamoClient.PutItemAsync("my-table", new MyItemType 
{ 
    Id = "123", 
    Name = "Test Item" 
});

// Get an item
var item = await dynamoClient.GetItemAsync<MyItemType>("my-table", "123");
```

## Documentation

For complete documentation, visit: https://github.com/cloudvelous/cloudvelous-aws-sdk
