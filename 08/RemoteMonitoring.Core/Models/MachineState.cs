namespace RemoteMonitoring.Core.Models;

/// <summary>
/// 机床实时状态。
/// </summary>
/// <summary>
/// 机床实时状态。
/// </summary>
/// <param name="DeviceId">设备编号</param>
/// <param name="DeviceName">设备名称</param>
/// <param name="Temperature">温度 (°C)</param>
/// <param name="Pressure">压力 (MPa)</param>
/// <param name="Speed">转速 (rpm)</param>
/// <param name="Production">累计产量</param>
/// <param name="IsOnline">是否在线</param>
/// <param name="Status">运行状态</param>
/// <param name="Timestamp">采样时间</param>
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
