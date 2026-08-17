using ModbusMonitor.Core.Communication;
using ModbusMonitor.Core.Services;

namespace ModbusMonitor.Tests.Services;

public sealed class DeviceReaderTests
{
    [Fact]
    public async Task ReadAsync_ConvertsFixedRegistersAndForwardsSlaveAddress()
    {
        await using var client = new FixedRegisterClient([2650, 123, 1500, 42]);
        var reader = new DeviceReader(client, slaveAddress: 7);

        var data = await reader.ReadAsync();

        Assert.Equal(26.5, data.Temperature);
        Assert.Equal(0.123, data.Pressure);
        Assert.Equal(1500, data.Speed);
        Assert.Equal(42, data.Production);
        Assert.True(data.IsConnected);
        Assert.Equal((byte)7, data.SlaveAddress);
        Assert.Equal((byte)7, client.LastSlaveAddress);
        Assert.Equal((ushort)0, client.LastStartAddress);
        Assert.Equal((ushort)4, client.LastRegisterCount);
    }

    [Fact]
    public async Task ReadAsync_WhenNotConnected_ThrowsInvalidOperation()
    {
        await using var client = new SimulatedModbusClient();
        var reader = new DeviceReader(client);

        await Assert.ThrowsAsync<InvalidOperationException>(() => reader.ReadAsync());
    }

    [Fact]
    public async Task ReadAsync_WhenRegisterResponseIsIncomplete_ThrowsInvalidDataException()
    {
        await using var client = new FixedRegisterClient([2650, 123, 1500]);
        var reader = new DeviceReader(client);

        var exception = await Assert.ThrowsAsync<InvalidDataException>(() => reader.ReadAsync());

        Assert.Contains("4", exception.Message);
        Assert.Contains("3", exception.Message);
    }

    [Fact]
    public async Task ReadAsync_MultipleReads_ProductionIncreases()
    {
        await using var client = new SimulatedModbusClient();
        await client.ConnectAsync();
        var reader = new DeviceReader(client);

        var data1 = await reader.ReadAsync();
        var data2 = await reader.ReadAsync();

        // 模拟器每次读取都会增加产量；严格大于才能发现“值未变化”的回归。
        Assert.True(data2.Production > data1.Production);
    }

    private sealed class FixedRegisterClient(ushort[] registers) : IModbusClient
    {
        public bool IsConnected => true;
        public byte? LastSlaveAddress { get; private set; }
        public ushort? LastStartAddress { get; private set; }
        public ushort? LastRegisterCount { get; private set; }

        public Task ConnectAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DisconnectAsync() => Task.CompletedTask;

        public Task<ushort[]> ReadHoldingRegistersAsync(
            byte slaveAddress,
            ushort startAddress,
            ushort count,
            CancellationToken cancellationToken = default)
        {
            LastSlaveAddress = slaveAddress;
            LastStartAddress = startAddress;
            LastRegisterCount = count;
            return Task.FromResult(registers);
        }

        public Task WriteSingleRegisterAsync(
            byte slaveAddress,
            ushort registerAddress,
            ushort value,
            CancellationToken cancellationToken = default) => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
