using ModbusMonitor.Core.Communication;
using ModbusMonitor.Core.Models;

namespace ModbusMonitor.Core.Services;

/// <summary>
/// 设备数据读取器 —— 从 Modbus 设备读取寄存器并转换为业务数据。
/// </summary>
public sealed class DeviceReader
{
    private readonly IModbusClient _client;
    private readonly byte _slaveAddress;

    public DeviceReader(IModbusClient client, byte slaveAddress = 1)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _slaveAddress = slaveAddress;
    }

    /// <summary>
    /// 读取设备数据（保持寄存器 0-3）。
    /// 寄存器映射：
    ///   0 = 温度 * 100
    ///   1 = 压力 * 1000
    ///   2 = 转速 (rpm)
    ///   3 = 产量
    /// </summary>
    public async Task<DeviceData> ReadAsync(CancellationToken cancellationToken = default)
    {
        var registers = await _client.ReadHoldingRegistersAsync(_slaveAddress, 0, 4, cancellationToken);

        // 协议响应长度属于外部输入。先给出带上下文的错误，避免稍后以数组越界掩盖设备响应异常。
        if (registers.Length < 4)
            throw new InvalidDataException($"读取设备数据需要 4 个寄存器，实际收到 {registers.Length} 个。");

        return new DeviceData(
            SlaveAddress: _slaveAddress,
            Temperature: registers[0] / 100.0,
            Pressure: registers[1] / 1000.0,
            Speed: registers[2],
            Production: registers[3],
            IsConnected: _client.IsConnected,
            Timestamp: DateTime.Now);
    }
}
