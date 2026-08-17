using DeviceDataGenerator.Models;
using DeviceDataGenerator.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DeviceDataGenerator.Tests;

public sealed class DeviceDataGeneratorServiceTests
{
    [Fact]
    public async Task StartAsync_WhenManyCallersStartTogether_OnlyOneRunStarts()
    {
        using var cancellation = new CancellationTokenSource();
        using var startBarrier = new Barrier(65);
        using var service = CreateService();
        var startedCount = 0;
        service.StatusChanged += (_, status) =>
        {
            if (status == "数据采集启动")
                Interlocked.Increment(ref startedCount);
        };

        var callers = Enumerable.Range(0, 64)
            .Select(_ => Task.Factory.StartNew(
                async () =>
                {
                    startBarrier.SignalAndWait();
                    await service.StartAsync(cancellation.Token);
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap())
            .ToArray();

        startBarrier.SignalAndWait();
        await Task.Delay(100);
        cancellation.Cancel();
        await Task.WhenAll(callers);

        Assert.Equal(1, startedCount);
    }

    [Fact]
    public async Task StartAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        var service = CreateService();
        service.Dispose();
        using var alreadyCancelled = new CancellationTokenSource();
        alreadyCancelled.Cancel();

        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => service.StartAsync(alreadyCancelled.Token));
    }

    [Fact]
    public async Task Dispose_WhileRunning_CancelsAndLetsOwnedRunReleaseResources()
    {
        var firstReading = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var service = CreateService();
        service.DataReceived += (_, _) => firstReading.TrySetResult();
        var run = service.StartAsync();
        await firstReading.Task.WaitAsync(TimeSpan.FromSeconds(2));

        service.Dispose();
        await run.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.False(service.IsRunning);
    }

    [Fact]
    public async Task StartAsync_WhenSubscriberThrowsUnrelatedCancellation_DoesNotSwallowFailure()
    {
        using var service = CreateService();
        service.DataReceived += (_, _) => throw new OperationCanceledException("订阅者自身取消");

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => service.StartAsync());

        Assert.Equal("订阅者自身取消", exception.Message);
    }

    [Fact]
    public async Task StartAsync_DoesNotAllowNextRunBeforePreviousStopEventIsPublished()
    {
        using var releaseStopEvent = new ManualResetEventSlim();
        var stopEventEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var service = new BlockingFirstStopService(stopEventEntered, releaseStopEvent);
        var startedCount = 0;
        service.StatusChanged += (_, status) =>
        {
            if (status == "数据采集启动")
                Interlocked.Increment(ref startedCount);
        };
        using var firstCancellation = new CancellationTokenSource();
        firstCancellation.Cancel();
        var firstRun = Task.Run(() => service.StartAsync(firstCancellation.Token));
        await stopEventEntered.Task.WaitAsync(TimeSpan.FromSeconds(2));

        using var secondCancellation = new CancellationTokenSource();
        secondCancellation.Cancel();
        await service.StartAsync(secondCancellation.Token);

        releaseStopEvent.Set();
        await firstRun.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.Equal(1, startedCount);
    }

    private static DeviceDataGeneratorService CreateService() => new(
        new SensorConfig(),
        NullLogger<DeviceDataGeneratorService>.Instance);

    private sealed class BlockingFirstStopService(
        TaskCompletionSource stopEventEntered,
        ManualResetEventSlim releaseStopEvent)
        : DeviceDataGeneratorService(
            new SensorConfig(),
            NullLogger<DeviceDataGeneratorService>.Instance)
    {
        private int _hasBlocked;

        protected override void OnStatusChanged(string status)
        {
            if (status == "数据采集停止" && Interlocked.Exchange(ref _hasBlocked, 1) == 0)
            {
                stopEventEntered.TrySetResult();
                releaseStopEvent.Wait(TimeSpan.FromSeconds(2));
            }

            base.OnStatusChanged(status);
        }
    }
}
