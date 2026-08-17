using IntegrationTest.Core;

namespace IntegrationTest.Tests;

public sealed class TestRunnerTests
{
    [Fact]
    public async Task RunAsync_WithAllStepsPassing_ReturnsSuccess()
    {
        var runner = new TestRunner()
            .AddSetup(async () => { await Task.CompletedTask; })
            .AddTest(async () => { await Task.CompletedTask; return true; })
            .AddTest(async () => { await Task.CompletedTask; return true; })
            .AddCleanup(async () => { await Task.CompletedTask; });

        var result = await runner.RunAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.TestsPassed);
        Assert.Equal(0, result.TestsFailed);
        Assert.Equal(1, result.SetupStepsCompleted);
        Assert.Equal(1, result.CleanupStepsCompleted);
    }

    [Fact]
    public async Task RunAsync_WithFailingTest_ReturnsFailure()
    {
        var runner = new TestRunner()
            .AddTest(async () => { await Task.CompletedTask; return true; })
            .AddTest(async () => { await Task.CompletedTask; return false; });

        var result = await runner.RunAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(1, result.TestsPassed);
        Assert.Equal(1, result.TestsFailed);
    }

    [Fact]
    public async Task RunAsync_WithException_CapturesError()
    {
        var runner = new TestRunner()
            .AddTest(async () =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException("Test error");
            });

        var result = await runner.RunAsync();

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains("Test error", result.Errors[0]);
    }

    [Fact]
    public async Task RunAsync_MeasuresDuration()
    {
        var runner = new TestRunner()
            .AddTest(async () =>
            {
                await Task.Delay(100);
                return true;
            });

        var result = await runner.RunAsync();

        Assert.True(result.Duration.TotalMilliseconds >= 100);
    }
}
