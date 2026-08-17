using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.Core.Services;

/// <summary>
/// 后台定时采集服务，支持协作式取消。
/// Java 对比：类似 ScheduledExecutorService + CancellationToken，
/// C# 用 Task.Delay + CancellationToken 实现非阻塞等待。
/// </summary>
public sealed class AcquisitionService
{
    private readonly FakeDataGenerator _generator;
    private readonly TimeSpan _interval;

    public AcquisitionService(FakeDataGenerator generator, TimeSpan? interval = null)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _interval = interval ?? TimeSpan.FromSeconds(1);
        if (_interval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(interval), "采样间隔必须为正数");
    }

    /// <summary>
    /// 在后台循环采集数据，直到 CancellationToken 被取消。
    /// </summary>
    /// <param name="onReading">每次采样后的异步回调</param>
    /// <param name="cancellationToken">取消令牌</param>
    public Task RunAsync(Func<DeviceReading, Task> onReading, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(onReading);

        return Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var reading = _generator.Generate();
                await onReading(reading).ConfigureAwait(false);

                await Task.Delay(_interval, cancellationToken).ConfigureAwait(false);
            }
        }, cancellationToken);
    }
}
