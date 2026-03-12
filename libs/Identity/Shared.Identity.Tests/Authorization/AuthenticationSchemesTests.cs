using Shared.Identity.Authorization;

namespace Shared.Identity.Tests.Authorization;

public class AuthenticationSchemesTests
{
    [Fact]
    public void Custom_ShouldHaveExpectedValue()
    {
        // Arrange & Act
        var value = AuthenticationSchemes.Custom;

        // Assert
        Assert.Equal("Custom", value);
    }

    [Fact]
    public void Public_ShouldHaveExpectedValue()
    {
        // Arrange & Act
        var value = AuthenticationSchemes.Public;

        // Assert
        Assert.Equal("Public", value);
    }

    [Fact]
    public void Internal_ShouldHaveExpectedValue()
    {
        // Arrange & Act
        var value = AuthenticationSchemes.Internal;

        // Assert
        Assert.Equal("Internal", value);
    }

    [Fact]
    public void AllSchemes_ShouldBeDistinct()
    {
        // Arrange
        var schemes = new[]
        {
            AuthenticationSchemes.Custom,
            AuthenticationSchemes.Public,
            AuthenticationSchemes.Internal,
        };

        // Act
        var distinct = schemes.Distinct();

        // Assert
        Assert.Equal(schemes.Length, distinct.Count());
    }
}
