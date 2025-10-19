# Installation Guide

This guide provides detailed instructions for installing and configuring the Cloudvelous AWS SDK packages.

## Prerequisites

- .NET 8.0 SDK or later
- GitHub account with access to the Cloudvelous organization
- GitHub Personal Access Token (PAT) with `read:packages` scope

## Step 1: Create a GitHub Personal Access Token

1. Navigate to GitHub Settings: https://github.com/settings/tokens
2. Click **"Developer settings"** → **"Personal access tokens"** → **"Tokens (classic)"**
3. Click **"Generate new token (classic)"**
4. Configure the token:
   - **Note:** `Cloudvelous NuGet Packages`
   - **Expiration:** Choose an appropriate duration (90 days recommended)
   - **Scopes:** Select `read:packages`
5. Click **"Generate token"**
6. **Important:** Copy the token immediately (you won't be able to see it again)

## Step 2: Configure NuGet Authentication

You have three options for configuring authentication:

### Option A: Global Configuration (Recommended for Development)

This configures authentication globally on your machine:

```bash
dotnet nuget add source https://nuget.pkg.github.com/sl-cloud/index.json \
  --name PrivateFeed \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text
```

**For Windows (with Credential Manager):**
```bash
dotnet nuget add source https://nuget.pkg.github.com/sl-cloud/index.json \
  --name PrivateFeed \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN
```

### Option B: Project-Level Configuration

Add a `nuget.config` file to your project root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="PrivateFeed" value="https://nuget.pkg.github.com/sl-cloud/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

Then authenticate:
```bash
dotnet nuget update source PrivateFeed \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text
```

### Option C: CI/CD Environment Variables

For CI/CD pipelines (GitHub Actions, Azure DevOps, etc.):

**GitHub Actions:**
```yaml
- name: Authenticate to GitHub Packages
  run: |
    dotnet nuget add source https://nuget.pkg.github.com/sl-cloud/index.json \
      --name PrivateFeed \
      --username ${{ github.actor }} \
      --password ${{ secrets.GITHUB_TOKEN }} \
      --store-password-in-clear-text
```

**Azure DevOps:**
```yaml
- task: DotNetCoreCLI@2
  displayName: 'Authenticate to GitHub Packages'
  inputs:
    command: 'custom'
    custom: 'nuget'
    arguments: 'add source https://nuget.pkg.github.com/sl-cloud/index.json --name PrivateFeed --username $(GitHubUsername) --password $(GitHubToken) --store-password-in-clear-text'
```

## Step 3: Verify Configuration

Check that the feed is configured correctly:

```bash
dotnet nuget list source
```

You should see output similar to:
```
Registered Sources:
  1.  nuget.org [Enabled]
      https://api.nuget.org/v3/index.json
  2.  PrivateFeed [Enabled]
      https://nuget.pkg.github.com/sl-cloud/index.json
```

## Step 4: Install Packages

Now you can install the Cloudvelous AWS SDK packages:

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

## Step 5: Verify Installation

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

### Error: "Unable to load the service index for source"

**Cause:** Authentication failed or token is invalid.

**Solution:**
1. Verify your GitHub token has `read:packages` scope
2. Check that the token hasn't expired
3. Re-run the authentication command with a fresh token

### Error: "Package 'Cloudvelous.Aws.Core' is not found"

**Cause:** Package hasn't been published yet or you don't have access.

**Solution:**
1. Verify the package has been published to GitHub Packages
2. Check that you have access to the Cloudvelous organization
3. Ensure you're using the correct feed URL

### Error: "401 Unauthorized"

**Cause:** Credentials are not being sent or are incorrect.

**Solution:**
1. Clear NuGet cache: `dotnet nuget locals all --clear`
2. Remove and re-add the source with fresh credentials
3. On Windows, check Windows Credential Manager for stored credentials

### Clearing Cached Credentials

If you need to update your credentials:

```bash
# List all sources
dotnet nuget list source

# Remove the old source
dotnet nuget remove source PrivateFeed

# Clear all NuGet caches
dotnet nuget locals all --clear

# Re-add with new credentials
dotnet nuget add source https://nuget.pkg.github.com/sl-cloud/index.json \
  --name PrivateFeed \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_NEW_GITHUB_TOKEN \
  --store-password-in-clear-text
```

## Security Best Practices

### For Development Machines

1. **Use short-lived tokens:** Set token expiration to 90 days or less
2. **Rotate tokens regularly:** Create a calendar reminder to rotate tokens
3. **Use separate tokens:** Don't reuse tokens across different purposes
4. **Store securely:** Consider using a password manager to store tokens

### For CI/CD Pipelines

1. **Use environment secrets:** Store tokens in GitHub Secrets, Azure Key Vault, etc.
2. **Limit scope:** Only grant `read:packages` scope (not `write:packages` unless needed)
3. **Use service accounts:** Create dedicated service accounts for CI/CD
4. **Audit access:** Regularly review which services have access to packages

### For Production Deployments

1. **Use managed identities:** When possible, use AWS IAM roles or Azure Managed Identities
2. **Avoid embedding tokens:** Never commit tokens to source control
3. **Use secret scanning:** Enable GitHub secret scanning to detect leaked tokens
4. **Implement rotation:** Automate token rotation where possible

## Alternative: Using Docker

If you're using Docker, you can configure NuGet authentication in your Dockerfile:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG GITHUB_TOKEN
WORKDIR /src

# Configure NuGet source
RUN dotnet nuget add source https://nuget.pkg.github.com/sl-cloud/index.json \
    --name PrivateFeed \
    --username ${GITHUB_USERNAME} \
    --password ${GITHUB_TOKEN} \
    --store-password-in-clear-text

# Copy and restore
COPY ["MyApp/MyApp.csproj", "MyApp/"]
RUN dotnet restore "MyApp/MyApp.csproj"

# Build
COPY . .
WORKDIR "/src/MyApp"
RUN dotnet build "MyApp.csproj" -c Release -o /app/build
```

Build with:
```bash
docker build \
  --build-arg GITHUB_USERNAME=your_username \
  --build-arg GITHUB_TOKEN=your_token_here \
  -t myapp .
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
- [Memory Bank](.cursor/memory-bank/memory-bank.md) - Comprehensive system documentation

