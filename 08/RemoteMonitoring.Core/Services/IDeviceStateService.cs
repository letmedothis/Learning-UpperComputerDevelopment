using RemoteMonitoring.Core.Models;

namespace RemoteMonitoring.Core.Services;

/// <summary>
/// 设备状态服务接口。
/// </summary>
public interface IDeviceStateService
{
    /// <summary>获取指定设备的当前状态。</summary>
    Task<MachineState?> GetCurrentStateAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>获取指定设备的最近 N 条历史状态。</summary>
    Task<IReadOnlyList<MachineState>> GetStateHistoryAsync(string deviceId, int count, CancellationToken cancellationToken = default);

    /// <summary>获取所有设备的摘要信息。</summary>
    Task<IReadOnlyList<DeviceSummary>> GetAllDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>更新设备状态。</summary>
    Task UpdateStateAsync(MachineState state, CancellationToken cancellationToken = default);
}
