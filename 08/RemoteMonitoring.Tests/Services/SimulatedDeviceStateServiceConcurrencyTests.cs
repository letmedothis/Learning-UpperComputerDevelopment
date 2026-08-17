using RemoteMonitoring.Core.Models;
using RemoteMonitoring.Core.Services;

namespace RemoteMonitoring.Tests.Services;

public sealed class SimulatedDeviceStateServiceConcurrencyTests
{
    [Fact]
    public async Task UpdateStateAsync_ConcurrentFirstWrites_PreservesEveryRecord()
    {
        const string deviceId = "RACE-001";
        const int writerCount = 64;
        var service = new SimulatedDeviceStateService();
        using var startBarrier = new Barrier(writerCount + 1);

        var writers = Enumerable.Range(0, writerCount)
            .Select(index => Task.Factory.StartNew(
                () =>
                {
                    startBarrier.SignalAndWait();
                    service.UpdateStateAsync(CreateState(deviceId, index)).GetAwaiter().GetResult();
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default))
            .ToArray();

        startBarrier.SignalAndWait();
        await Task.WhenAll(writers);

        var history = await service.GetStateHistoryAsync(deviceId, writerCount);

        Assert.Equal(writerCount, history.Count);
        Assert.Equal(writerCount, history.Select(state => state.Production).Distinct().Count());
    }

    [Fact]
    public async Task UpdateAndReadConcurrently_ReturnsStableSnapshotsAndKeepsNewestThousand()
    {
        const string deviceId = "CNC-001";
        const int writerCount = 8;
        const int writesPerWriter = 500;
        var service = new SimulatedDeviceStateService();

        var writers = Enumerable.Range(0, writerCount)
            .Select(writer => Task.Run(async () =>
            {
                for (var index = 0; index < writesPerWriter; index++)
                {
                    var sequence = writer * writesPerWriter + index;
                    await service.UpdateStateAsync(CreateState(deviceId, sequence));
                }
            }));

        var readers = Enumerable.Range(0, writerCount)
            .Select(_ => Task.Run(async () =>
            {
                for (var index = 0; index < writesPerWriter; index++)
                {
                    var snapshot = await service.GetStateHistoryAsync(deviceId, 1000);
                    _ = snapshot.Count;
                }
            }));

        await Task.WhenAll(writers.Concat(readers));

        var history = await service.GetStateHistoryAsync(deviceId, 2000);
        Assert.Equal(1000, history.Count);
        Assert.Equal(1000, history.Select(state => state.Production).Distinct().Count());
    }

    [Fact]
    public async Task GetStateHistoryAsync_ReturnedSnapshotDoesNotChangeAfterUpdate()
    {
        const string deviceId = "SNAPSHOT-001";
        var service = new SimulatedDeviceStateService();
        await service.UpdateStateAsync(CreateState(deviceId, 1));

        var snapshot = await service.GetStateHistoryAsync(deviceId, 1000);
        await service.UpdateStateAsync(CreateState(deviceId, 2));

        Assert.Single(snapshot);
        Assert.Equal(1, snapshot[0].Production);
    }

    private static MachineState CreateState(string deviceId, int sequence)
    {
        return new MachineState(
            DeviceId: deviceId,
            DeviceName: deviceId,
            Temperature: sequence,
            Pressure: sequence,
            Speed: sequence,
            Production: sequence,
            IsOnline: true,
            Status: "运行中",
            Timestamp: DateTime.UnixEpoch.AddMilliseconds(sequence));
    }
}
