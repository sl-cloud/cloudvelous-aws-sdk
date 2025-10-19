# Cloudvelous.Aws.OpenSearch

AWS OpenSearch client for search and analytics operations.

## Features

- **Search Operations**: Perform search queries and analytics
- **Type Safety**: Strongly-typed query and response objects
- **Retry Policies**: Automatic retry with exponential backoff
- **Dependency Injection**: Easy integration with Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package Cloudvelous.Aws.OpenSearch
```

## Quick Start

```csharp
using Cloudvelous.Aws.OpenSearch;
using Microsoft.Extensions.DependencyInjection;

// Register services
services.AddOpenSearch();

// Use the client
var openSearchClient = serviceProvider.GetRequiredService<IOpenSearchClient>();

// Perform search operations
var results = await openSearchClient.SearchAsync<MyDocumentType>(
    "my-index", 
    new SearchQuery { Query = "test" }
);
```

## Documentation

For complete documentation, visit: https://github.com/cloudvelous/cloudvelous-aws-sdk
