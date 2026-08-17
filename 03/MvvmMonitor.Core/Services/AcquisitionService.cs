using MvvmMonitor.Core.Models;

namespace MvvmMonitor.Core.Services;

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
