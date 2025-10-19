# Installation Guide

This guide provides detailed instructions for installing and configuring the Cloudvelous AWS SDK packages.

## Prerequisites

- .NET 8.0 SDK or later

## Installation

The Cloudvelous AWS SDK is available as a public package on NuGet.org. No authentication is required.

### Install Packages

Install the Cloudvelous AWS SDK packages using the .NET CLI:

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

**Using Visual Studio:**
1. Right-click on your project in Solution Explorer
2. Select "Manage NuGet Packages"
3. Search for "Cloudvelous.Aws"
4. Install the packages you need

**Using Package Manager Console:**
```powershell
Install-Package Cloudvelous.Aws.Core
Install-Package Cloudvelous.Aws.SecretsManager
# ... etc
```

## Verify Installation

Create a simple test to verify the packages are installed correctly:

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
        });
    })
    .Build();

Console.WriteLine("✅ Cloudvelous AWS SDK installed successfully!");
```

Run the test:
```bash
dotnet run
```

## Troubleshooting

### Error: "Package 'Cloudvelous.Aws.Core' is not found"

**Cause:** Package hasn't been published to NuGet.org yet.

**Solution:**
1. Verify the package has been published to NuGet.org
2. Check the package name spelling
3. Try refreshing the NuGet cache: `dotnet nuget locals all --clear`

### Error: "Unable to load the service index for source"

**Cause:** Network connectivity issues or NuGet.org is down.

**Solution:**
1. Check your internet connection
2. Verify NuGet.org is accessible: https://www.nuget.org/
3. Try clearing the NuGet cache: `dotnet nuget locals all --clear`

### Clearing NuGet Cache

If you encounter package resolution issues:

```bash
# Clear all NuGet caches
dotnet nuget locals all --clear

# Try restoring again
dotnet restore
```

## Using Docker

If you're using Docker, here's a simple Dockerfile example:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore
COPY ["MyApp/MyApp.csproj", "MyApp/"]
RUN dotnet restore "MyApp/MyApp.csproj"

# Build
COPY . .
WORKDIR "/src/MyApp"
RUN dotnet build "MyApp.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "MyApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

Build with:
```bash
docker build -t myapp .
```

## Getting Help

If you encounter issues not covered in this guide:

1. Check the [main README](README.md) for general information
2. Review the [troubleshooting section](#troubleshooting) above
3. Search existing [GitHub Issues](https://github.com/cloudvelous/cloudvelous-aws-sdk/issues)
4. Create a new issue with:
   - Your OS and .NET version
   - The exact error message
   - Steps to reproduce
   - Relevant configuration (with tokens redacted)

## Next Steps

Once installed, check out:

- [README.md](README.md) - Quick start and usage examples
- [Package READMEs](src/) - Service-specific documentation
- [Sample Application](samples/Cloudvelous.Aws.Samples.Console/) - Working examples

