using Cloudvelous.Aws.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Cloudvelous.Aws.Core.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAwsCore_WithNullConfigureOptions_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services

        // Act
        services.AddAwsCore();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var exceptionHandler = serviceProvider.GetService<IAwsExceptionHandler>();
        exceptionHandler.Should().NotBeNull();
        exceptionHandler.Should().BeOfType<AwsExceptionHandlerService>();
    }

    [Fact]
    public void AddAwsCore_WithConfigureOptions_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var expectedRegion = "us-west-2";

        // Act
        services.AddAwsCore(options =>
        {
            options.Region = expectedRegion;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AwsClientOptions>>();
        options.Value.Region.Should().Be(expectedRegion);
    }

    [Fact]
    public void AddAwsCore_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddAwsCore();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddAwsCore_ShouldRegisterExceptionHandlerAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging services

        // Act
        services.AddAwsCore();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var handler1 = serviceProvider.GetRequiredService<IAwsExceptionHandler>();
        var handler2 = serviceProvider.GetRequiredService<IAwsExceptionHandler>();
        
        handler1.Should().BeSameAs(handler2);
    }
}
