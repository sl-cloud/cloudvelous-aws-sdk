# Cloudvelous.Aws.Lambda

AWS Lambda client for serverless function invocation.

## Features

- **Function Invocation**: Invoke Lambda functions synchronously and asynchronously
- **Type Safety**: Strongly-typed payload serialization/deserialization
- **Retry Policies**: Automatic retry with exponential backoff
- **Dependency Injection**: Easy integration with Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package Cloudvelous.Aws.Lambda
```

## Quick Start

```csharp
using Cloudvelous.Aws.Lambda;
using Microsoft.Extensions.DependencyInjection;

// Register services
services.AddLambda();

// Use the client
var lambdaClient = serviceProvider.GetRequiredService<ILambdaClient>();

// Invoke a function
var response = await lambdaClient.InvokeAsync<MyRequestType, MyResponseType>(
    "my-function-name", 
    new MyRequestType { Data = "test" }
);
```

## Documentation

For complete documentation, visit: https://github.com/sl-cloud/cloudvelous-aws-sdk
