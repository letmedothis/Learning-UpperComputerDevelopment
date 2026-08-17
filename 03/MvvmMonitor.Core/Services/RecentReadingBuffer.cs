using MvvmMonitor.Core.Models;

namespace MvvmMonitor.Core.Services;

public sealed class RecentReadingBuffer
{
    private readonly List<DeviceReading> _readings;
    private readonly int _capacity;

    public RecentReadingBuffer(int capacity = 20)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _capacity = capacity;
        _readings = new List<DeviceReading>(capacity);
    }

    public int Count => _readings.Count;
    public IReadOnlyList<DeviceReading> Snapshot => _readings.ToArray();

    public void Add(DeviceReading reading)
    {
        _readings.Insert(0, reading);
        if (_readings.Count > _capacity) _readings.RemoveAt(_readings.Count - 1);
    }

    public void Clear() => _readings.Clear();
}
