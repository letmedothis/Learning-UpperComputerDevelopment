namespace RemoteMonitoring.Core.Models;

/// <summary>
/// 设备摘要信息。
/// </summary>
public sealed record DeviceSummary(
    string DeviceId,
    string DeviceName,
    bool IsOnline,
    string Status,
    DateTime LastUpdate);
