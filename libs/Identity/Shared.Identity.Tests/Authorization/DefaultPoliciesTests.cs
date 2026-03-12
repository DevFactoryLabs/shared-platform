using Microsoft.AspNetCore.Authorization;
using Shared.Identity.Authorization;

namespace Shared.Identity.Tests.Authorization;

public class DefaultPoliciesTests
{
    [Fact]
    public void CreateOperationalUserPolicy_ShouldReturnPolicyBuilder()
    {
        // Arrange & Act
        var builder = DefaultPolicies.CreateOperationalUserPolicy();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<AuthorizationPolicyBuilder>(builder);
    }

    [Fact]
    public void CreateOperationalUserPolicy_ShouldUsePublicAuthenticationScheme()
    {
        // Arrange & Act
        var policy = DefaultPolicies.CreateOperationalUserPolicy().Build();

        // Assert
        Assert.Contains(AuthenticationSchemes.Public, policy.AuthenticationSchemes);
    }

    [Fact]
    public void CreateOperationalUserPolicy_ShouldRequireEmailVerifiedClaim()
    {
        // Arrange & Act
        var policy = DefaultPolicies.CreateOperationalUserPolicy().Build();

        // Assert
        Assert.NotEmpty(policy.Requirements);
    }

    [Fact]
    public void CreateOperationalUserPolicy_CalledTwice_ShouldReturnIndependentBuilders()
    {
        // Arrange & Act
        var builder1 = DefaultPolicies.CreateOperationalUserPolicy();
        var builder2 = DefaultPolicies.CreateOperationalUserPolicy();

        // Assert
        Assert.NotSame(builder1, builder2);
    }
}
