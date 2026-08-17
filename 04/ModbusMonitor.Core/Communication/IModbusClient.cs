namespace ModbusMonitor.Core.Communication;

/// <summary>
/// Modbus 通信接口 —— 抽象出读写操作，便于测试和替换实现。
/// </summary>
public interface IModbusClient : IAsyncDisposable
{
    /// <summary>连接到 Modbus 设备</summary>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>断开连接</summary>
    Task DisconnectAsync();

    /// <summary>是否已连接</summary>
    bool IsConnected { get; }

    /// <summary>读取保持寄存器（功能码 0x03）</summary>
    Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort count, CancellationToken cancellationToken = default);

    /// <summary>写入单个寄存器（功能码 0x06）</summary>
    Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value, CancellationToken cancellationToken = default);
}
