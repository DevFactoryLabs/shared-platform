using Polly;
using Shared.Inbox.Resilience;

namespace Shared.Inbox.Tests.Resilience;

public class InboxResilienceTests
{
    [Fact]
    public void CreateDefault_ShouldReturnNonNullPipeline()
    {
        // Arrange & Act
        var pipeline = InboxResilience.CreateDefault();

        // Assert
        Assert.NotNull(pipeline);
    }

    [Fact]
    public async Task CreateDefault_ShouldCreatePipelineThatExecutesSuccessfully()
    {
        // Arrange
        var pipeline = InboxResilience.CreateDefault();
        var executed = false;

        // Act
        await pipeline.ExecuteAsync(_ =>
        {
            executed = true;
            return ValueTask.CompletedTask;
        });

        // Assert
        Assert.True(executed);
    }
}
