using Microsoft.Extensions.DependencyInjection;
using Shared.Correlation.Context;
using Shared.Correlation.Extensions;

namespace Shared.Correlation.Tests.Extensions;

public class CorrelationExtensionsTests
{
    [Fact]
    public void AddCorrelationContext_ShouldRegisterICorrelationContextAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCorrelationContext();

        // Assert
        var descriptor = Assert.Single(services, sd => sd.ServiceType == typeof(ICorrelationContext));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.Equal(typeof(CorrelationContext), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCorrelationContext_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddCorrelationContext();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void ToHeaders_WhenCorrelationIdIsSet_ShouldReturnDictionaryWithCorrelationIdKey()
    {
        // Arrange
        var context = new CorrelationContext();
        var correlationId = "test-correlation-id-123";
        context.SetCorrelationId(correlationId);

        // Act
        var headers = context.ToHeaders();

        // Assert
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("correlation_id"));
        Assert.Equal(correlationId, headers["correlation_id"]);
    }

    [Fact]
    public void ToHeaders_WhenCorrelationIdIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => context.ToHeaders());
    }
}
