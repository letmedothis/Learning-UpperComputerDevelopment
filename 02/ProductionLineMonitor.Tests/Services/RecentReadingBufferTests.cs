using ProductionLineMonitor.Core.Models;
using ProductionLineMonitor.Core.Services;

namespace ProductionLineMonitor.Tests.Services;

public sealed class RecentReadingBufferTests
{
    private static DeviceReading CreateReading(int production) =>
        new(DateTime.Now, 25, 0.3, 1500, production,
            MetricLevel.Normal, MetricLevel.Normal, MetricLevel.Normal, MetricLevel.Normal);

    [Fact]
    public void Add_WhenMoreThanCapacity_KeepsNewestTwentyInNewestFirstOrder()
    {
        var buffer = new RecentReadingBuffer(20);
        for (var index = 1; index <= 25; index++) buffer.Add(CreateReading(index));

        Assert.Equal(20, buffer.Count);
        Assert.Equal(Enumerable.Range(6, 20).Reverse(), buffer.Snapshot.Select(x => x.Production));
    }

    [Fact]
    public void Snapshot_CannotMutateInternalStorage()
    {
        var buffer = new RecentReadingBuffer(20);
        buffer.Add(CreateReading(1));
        var snapshot = buffer.Snapshot;

        buffer.Add(CreateReading(2));

        Assert.Single(snapshot);
        Assert.Equal(2, buffer.Count);
    }

    [Fact]
    public void Clear_RemovesAllReadings()
    {
        var buffer = new RecentReadingBuffer(20);
        buffer.Add(CreateReading(1));
        buffer.Clear();
        Assert.Empty(buffer.Snapshot);
    }

    [Fact]
    public void Constructor_WithZeroCapacity_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecentReadingBuffer(0));

    [Fact]
    public void Add_WhenUnderCapacity_CountIncreases()
    {
        var buffer = new RecentReadingBuffer(5);
        buffer.Add(CreateReading(1));
        buffer.Add(CreateReading(2));
        Assert.Equal(2, buffer.Count);
    }
}
