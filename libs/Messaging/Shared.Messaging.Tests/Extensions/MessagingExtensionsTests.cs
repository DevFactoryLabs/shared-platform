using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Connection;
using Shared.Messaging.Extensions;

namespace Shared.Messaging.Tests.Extensions;

public class MessagingExtensionsTests
{
    [Fact]
    public void AddMessagingServices_ShouldRegisterMessageBusAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMessagingServices();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMessageBus));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddMessagingServices_ShouldRegisterMessageBusConnectionFactoryAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMessagingServices();

        // Assert
        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IMessageBusConnectionFactory)
        );
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddMessagingServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMessagingServices();

        // Assert
        Assert.Same(services, result);
    }
}
