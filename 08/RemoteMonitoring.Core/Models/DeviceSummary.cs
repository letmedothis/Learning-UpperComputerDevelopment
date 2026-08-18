namespace RemoteMonitoring.Core.Models;

/// <summary>
/// 设备摘要信息。
/// </summary>
/// <summary>
/// 设备摘要信息。
/// </summary>
/// <param name="DeviceId">设备编号</param>
/// <param name="DeviceName">设备名称</param>
/// <param name="IsOnline">是否在线</param>
/// <param name="Status">当前状态描述</param>
/// <param name="LastUpdate">最后更新时间</param>
public sealed record DeviceSummary(
    string DeviceId,
    string DeviceName,
    bool IsOnline,
    string Status,
    DateTime LastUpdate);
