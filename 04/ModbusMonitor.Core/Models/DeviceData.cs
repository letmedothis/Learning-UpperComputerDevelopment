namespace ModbusMonitor.Core.Models;

/// <summary>
/// 从 Modbus 设备读取的数据点。
/// </summary>
public sealed record DeviceData(
    int SlaveAddress,
    double Temperature,      // 保持寄存器 0
    double Pressure,         // 保持寄存器 1
    double Speed,            // 保持寄存器 2
    int Production,          // 保持寄存器 3
    bool IsConnected,
    DateTime Timestamp);
