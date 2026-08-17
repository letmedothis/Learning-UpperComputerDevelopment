namespace RemoteMonitoring.Core.Models;

/// <summary>
/// 机床实时状态。
/// </summary>
public sealed record MachineState(
    string DeviceId,
    string DeviceName,
    double Temperature,
    double Pressure,
    double Speed,
    int Production,
    bool IsOnline,
    string Status,
    DateTime Timestamp);
