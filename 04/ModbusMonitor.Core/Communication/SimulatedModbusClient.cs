namespace ModbusMonitor.Core.Communication;

/// <summary>
/// 模拟 Modbus 客户端 —— 用于开发和测试，无需真实设备。
/// </summary>
public sealed class SimulatedModbusClient : IModbusClient
{
    private readonly Random _random = new();
    private readonly Dictionary<ushort, ushort> _registers = new();
    private bool _connected;
    private int _production;

    /// <summary>是否已连接。</summary>
    public bool IsConnected => _connected;

    /// <summary>模拟连接到设备。</summary>
    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _connected = true;
        return Task.CompletedTask;
    }

    /// <summary>模拟断开连接。</summary>
    public Task DisconnectAsync()
    {
        _connected = false;
        return Task.CompletedTask;
    }

    /// <summary>读取保持寄存器，未写入的地址返回模拟值。</summary>
    public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort count, CancellationToken cancellationToken = default)
    {
        if (!_connected) throw new InvalidOperationException("未连接到设备");

        var result = new ushort[count];
        for (ushort i = 0; i < count; i++)
        {
            var address = (ushort)(startAddress + i);
            result[i] = _registers.TryGetValue(address, out var value) ? value : GenerateSimulatedValue(address);
        }
        return Task.FromResult(result);
    }

    /// <summary>写入单个寄存器。</summary>
    public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value, CancellationToken cancellationToken = default)
    {
        if (!_connected) throw new InvalidOperationException("未连接到设备");
        _registers[registerAddress] = value;
        return Task.CompletedTask;
    }

    private ushort GenerateSimulatedValue(ushort address)
    {
        // 模拟不同寄存器的值
        return address switch
        {
            0 => (ushort)(2000 + _random.Next(0, 3000)),  // 温度 * 100
            1 => (ushort)(100 + _random.Next(0, 500)),     // 压力 * 1000
            2 => (ushort)(800 + _random.Next(0, 1200)),    // 转速
            3 => (ushort)(++_production),                   // 产量
            _ => (ushort)_random.Next(0, 1000)
        };
    }

    /// <summary>释放资源，断开连接。</summary>
    public ValueTask DisposeAsync()
    {
        _connected = false;
        return ValueTask.CompletedTask;
    }
}
