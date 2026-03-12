using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using Shared.Correlation.Context;
using Shared.Correlation.OpenTelemetry.Extensions;

namespace Shared.Correlation.OpenTelemetry.Tests.Extensions;

public class OpenTelemetryExtensionsTests
{
    [Fact]
    public void AddCorrelationLogProcessor_ShouldReturnSameOptions()
    {
        // Arrange
        var options = new OpenTelemetryLoggerOptions();

        // Act
        var result = options.AddCorrelationLogProcessor();

        // Assert
        Assert.NotNull(result);
        Assert.Same(options, result);
    }

    [Fact]
    public void AddCorrelationLogProcessor_WhenServiceProviderHasCorrelationContext_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ICorrelationContext, CorrelationContext>();
        var serviceProvider = services.BuildServiceProvider();

        var options = new OpenTelemetryLoggerOptions();

        // Act
        var exception = Record.Exception(() => options.AddCorrelationLogProcessor());

        // Assert
        Assert.Null(exception);
        Assert.NotNull(options);
    }
}
