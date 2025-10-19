# Cloudvelous.Aws.Sqs

AWS Simple Queue Service (SQS) client with batch operations and retry policies.

## Features

- **Queue Management**: Create, configure, and manage SQS queues
- **Message Operations**: Send, receive, and delete messages with batch support
- **Type Safety**: Strongly-typed message serialization/deserialization
- **Retry Policies**: Automatic retry with exponential backoff
- **Dependency Injection**: Easy integration with Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package Cloudvelous.Aws.Sqs
```

## Quick Start

```csharp
using Cloudvelous.Aws.Sqs;
using Microsoft.Extensions.DependencyInjection;

// Register services
services.AddSqs();

// Use the client
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
```

## Configuration

Configure SQS settings in `appsettings.json`:

```json
{
  "Sqs": {
    "DefaultVisibilityTimeoutSeconds": 30,
    "MaxReceiveMessages": 10
  }
}
```

## Documentation

For complete documentation, visit: https://github.com/sl-cloud/cloudvelous-aws-sdk
