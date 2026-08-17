using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.Core.Services;

/// <summary>
/// 固定容量的最近采样缓冲区，最新数据在最前。
/// Java 对比：类似 LinkedList + 固定大小，但 Snapshot 返回只读副本防止外部修改。
/// </summary>
public sealed class RecentReadingBuffer
{
    private readonly List<DeviceReading> _readings;
    private readonly int _capacity;

    public RecentReadingBuffer(int capacity = 20)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "容量必须大于 0");
        _capacity = capacity;
        _readings = new List<DeviceReading>(capacity);
    }

    public int Count => _readings.Count;

    /// <summary>
    /// 返回只读快照，后续 Add 不会影响已返回的列表。
    /// </summary>
    public IReadOnlyList<DeviceReading> Snapshot => _readings.ToArray();

    public void Add(DeviceReading reading)
    {
        _readings.Insert(0, reading);
        if (_readings.Count > _capacity)
            _readings.RemoveAt(_readings.Count - 1);
    }

    public void Clear() => _readings.Clear();
}
