using ProductionLineMonitor.Core.Models;
using ProductionLineMonitor.Core.Services;

namespace ProductionLineMonitor.Tests.Services;

public sealed class AcquisitionServiceTests
{
    [Fact]
    public async Task RunAsync_PublishesReadingsUntilCancelled()
    {
        var service = new AcquisitionService(new FakeDataGenerator(new Random(1)), TimeSpan.FromMilliseconds(10));
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        var firstReading = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var count = 0;

        var task = service.RunAsync(_ =>
        {
            Interlocked.Increment(ref count);
            firstReading.TrySetResult();
            return Task.CompletedTask;
        }, cts.Token);

        await firstReading.Task;
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        Assert.True(count >= 1);
    }

    [Fact]
    public async Task RunAsync_AfterCancellation_DoesNotPublishMoreReadings()
    {
        var service = new AcquisitionService(new FakeDataGenerator(new Random(2)), TimeSpan.FromMilliseconds(10));
        using var cts = new CancellationTokenSource();
        var firstReading = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var count = 0;
        var task = service.RunAsync(_ =>
        {
            Interlocked.Increment(ref count);
            firstReading.TrySetResult();
            return Task.CompletedTask;
        }, cts.Token);

        await firstReading.Task;
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        var countAfterCancel = count;
        await Task.Delay(40);
        Assert.Equal(countAfterCancel, count);
    }

    [Fact]
    public void Constructor_WithZeroInterval_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new AcquisitionService(new FakeDataGenerator(), TimeSpan.Zero));

    [Fact]
    public void Constructor_WithNullGenerator_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(
            () => new AcquisitionService(null!));
}
