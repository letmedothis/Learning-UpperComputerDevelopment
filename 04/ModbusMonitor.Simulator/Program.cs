using ModbusMonitor.Core.Communication;
using ModbusMonitor.Core.Services;

// Modbus 设备模拟器 —— 演示从模拟设备读取数据
Console.WriteLine("=== Modbus 设备模拟器 ===\n");

await using var client = new SimulatedModbusClient();
var reader = new DeviceReader(client);

Console.WriteLine("正在连接设备...");
await client.ConnectAsync();
Console.WriteLine($"已连接: {client.IsConnected}\n");

Console.WriteLine("开始读取数据（按 Ctrl+C 停止）...\n");
Console.WriteLine($"{"时间",-12} {"温度 °C",10} {"压力 MPa",10} {"转速 rpm",10} {"产量",8}");
Console.WriteLine(new string('-', 55));

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        var data = await reader.ReadAsync(cts.Token);
        Console.WriteLine($"{data.Timestamp:HH:mm:ss,-12} {data.Temperature,10:F2} {data.Pressure,10:F3} {data.Speed,10:F0} {data.Production,8}");
        await Task.Delay(1000, cts.Token);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("\n已停止读取");
}

await client.DisconnectAsync();
Console.WriteLine("已断开连接");
