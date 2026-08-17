using RemoteMonitoring.Core.Models;

namespace RemoteMonitoring.Core.Services;

/// <summary>
/// 设备状态服务接口。
/// </summary>
public interface IDeviceStateService
{
    Task<MachineState?> GetCurrentStateAsync(string deviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MachineState>> GetStateHistoryAsync(string deviceId, int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeviceSummary>> GetAllDevicesAsync(CancellationToken cancellationToken = default);
    Task UpdateStateAsync(MachineState state, CancellationToken cancellationToken = default);
}
