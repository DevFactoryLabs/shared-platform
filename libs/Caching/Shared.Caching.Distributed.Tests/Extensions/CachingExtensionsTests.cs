using Microsoft.Extensions.DependencyInjection;
using Shared.Caching;
using Shared.Caching.Distributed.Extensions;

namespace Shared.Caching.Distributed.Tests.Extensions;

public class CachingExtensionsTests
{
    [Fact]
    public void AddDistributedCaching_ShouldRegisterCacheServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDistributedCaching("localhost:6379");

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICacheService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddDistributedCaching_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDistributedCaching("localhost:6379");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddDistributedCaching_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDistributedCaching("localhost:6379");

        // Assert
        Assert.Same(services, result);
    }
}
