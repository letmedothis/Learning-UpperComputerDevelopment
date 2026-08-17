using System.Collections.Concurrent;
using RemoteMonitoring.Core.Models;

namespace RemoteMonitoring.Core.Services;

/// <summary>
/// 模拟设备状态服务 —— 用于开发和测试。
/// </summary>
public sealed class SimulatedDeviceStateService : IDeviceStateService
{
    private readonly ConcurrentDictionary<string, MachineState> _currentStates = new();
    // ConcurrentDictionary 只保护键级操作；每台设备的可变历史由容器内部的锁独立保护。
    private readonly ConcurrentDictionary<string, DeviceHistory> _history = new();
    private readonly Random _random = new();

    public SimulatedDeviceStateService()
    {
        // 初始化模拟设备
        InitializeDevice("CNC-001", "数控机床 #1");
        InitializeDevice("CNC-002", "数控机床 #2");
        InitializeDevice("CNC-003", "数控机床 #3");
    }

    private void InitializeDevice(string deviceId, string deviceName)
    {
        var state = GenerateSimulatedState(deviceId, deviceName);
        _currentStates[deviceId] = state;
        _history[deviceId] = new DeviceHistory(state);
    }

    public Task<MachineState?> GetCurrentStateAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        _currentStates.TryGetValue(deviceId, out var state);
        return Task.FromResult(state);
    }

    public Task<IReadOnlyList<MachineState>> GetStateHistoryAsync(string deviceId, int count, CancellationToken cancellationToken = default)
    {
        if (!_history.TryGetValue(deviceId, out var history))
            return Task.FromResult<IReadOnlyList<MachineState>>(Array.Empty<MachineState>());

        return Task.FromResult(history.GetSnapshot(count));
    }

    public Task<IReadOnlyList<DeviceSummary>> GetAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        var summaries = _currentStates.Values
            .Select(s => new DeviceSummary(s.DeviceId, s.DeviceName, s.IsOnline, s.Status, s.Timestamp))
            .ToList();

        return Task.FromResult<IReadOnlyList<DeviceSummary>>(summaries);
    }

    public Task UpdateStateAsync(MachineState state, CancellationToken cancellationToken = default)
    {
        _currentStates[state.DeviceId] = state;

        var history = _history.GetOrAdd(state.DeviceId, static _ => new DeviceHistory());
        history.Add(state);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 生成模拟设备状态。
    /// </summary>
    public MachineState GenerateSimulatedState(string deviceId, string deviceName)
    {
        return new MachineState(
            DeviceId: deviceId,
            DeviceName: deviceName,
            Temperature: Math.Round(20 + _random.NextDouble() * 25, 1),
            Pressure: Math.Round(0.1 + _random.NextDouble() * 0.4, 3),
            Speed: Math.Round(800 + _random.NextDouble() * 1200, 1),
            Production: _random.Next(0, 10000),
            IsOnline: _random.Next(0, 10) > 0, // 90% 在线
            Status: _random.Next(0, 4) switch
            {
                0 => "运行中",
                1 => "空闲",
                2 => "维护中",
                _ => "报警"
            },
            Timestamp: DateTime.Now);
    }

    private sealed class DeviceHistory
    {
        private const int Capacity = 1000;
        private readonly object _syncRoot = new();
        private readonly List<MachineState> _states = new(Capacity);

        public DeviceHistory(MachineState? initialState = null)
        {
            if (initialState is not null)
                _states.Add(initialState);
        }

        public void Add(MachineState state)
        {
            lock (_syncRoot)
            {
                _states.Add(state);
                if (_states.Count > Capacity)
                    _states.RemoveRange(0, _states.Count - Capacity);
            }
        }

        public IReadOnlyList<MachineState> GetSnapshot(int count)
        {
            lock (_syncRoot)
            {
                return _states
                    .OrderByDescending(state => state.Timestamp)
                    .Take(count)
                    .ToArray();
            }
        }
    }
}
