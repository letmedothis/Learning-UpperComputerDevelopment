using DeviceDataGenerator.Models;
using DeviceDataGenerator.Services;
using DeviceDataGenerator.Utils;
using Microsoft.Extensions.Logging;

// 创建日志工厂
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});

// 创建传感器配置
var config = new SensorConfig
{
    TemperatureBase = 25.0,
    TemperatureRange = 10.0,
    PressureBase = 0.3,
    PressureRange = 0.2,
    SpeedBase = 1500,
    SpeedRange = 500
};

Console.WriteLine("=== 设备数据生成器 ===");
Console.WriteLine("按 Ctrl+C 停止数据采集\n");

// 创建数据生成器和分析器
using var generator = new DeviceDataGeneratorService(
    config,
    loggerFactory.CreateLogger<DeviceDataGeneratorService>()
);
var analyzer = new DataAnalyzer(maxSamples: 60);

// 订阅事件
generator.DataReceived += (sender, reading) =>
{
    // 添加到分析器
    analyzer.AddReading(reading);

    // 显示数据
    Console.WriteLine($"  {reading}");

    // 每 10 条数据显示一次统计
    if (analyzer.SampleCount % 10 == 0)
    {
        PrintAnalysis();
    }
};

generator.StatusChanged += (sender, status) =>
{
    Console.WriteLine($"\n[状态] {status}\n");
};

// 创建取消令牌源（Ctrl+C 取消）
using var cts = new CancellationTokenSource();

// 捕获 Ctrl+C 信号
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true; // 阻止进程立即退出
    Console.WriteLine("\n正在停止...");
    cts.Cancel();
};

// 显示工具类示例
Console.WriteLine("--- 工具类示例 ---");
Console.WriteLine($"  25°C = {SensorUtils.CelsiusToFahrenheit(25):F1}°F");
Console.WriteLine($"  0.3 MPa = {SensorUtils.MpaToPsi(0.3):F1} PSI");
Console.WriteLine($"  温度 25°C 状态: {SensorUtils.GetTemperatureStatus(25)}");
Console.WriteLine($"  压力 0.3 MPa 状态: {SensorUtils.GetPressureStatus(0.3)}");
Console.WriteLine($"  转速 1500 rpm 状态: {SensorUtils.GetSpeedStatus(1500)}");
Console.WriteLine();

try
{
    // 启动数据采集（会一直运行直到取消）
    await generator.StartAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // 正常取消，忽略异常
}

// 显示最终统计
Console.WriteLine("\n=== 最终统计 ===");
PrintAnalysis();

// 显示温度分布
Console.WriteLine("\n=== 温度分布 ===");
var distribution = analyzer.GroupByTemperatureRange(5.0);
foreach (var (range, count) in distribution)
{
    Console.WriteLine($"  {range}: {new string('■', count)} ({count})");
}

// 演示倍率转换
Console.WriteLine("\n=== 倍率转换示例 ===");
double rawSensorValue = 1024;
double convertedValue = SensorUtils.ApplyMultiplier(rawSensorValue, 0.1, -50);
Console.WriteLine($"  原始传感器值: {rawSensorValue}");
Console.WriteLine($"  转换后温度: {convertedValue:F1}°C");

return;

// 打印分析结果
void PrintAnalysis()
{
    Console.WriteLine($"\n--- 统计 (最近 {analyzer.SampleCount} 条数据) ---");
    Console.WriteLine($"  {analyzer.AnalyzeTemperature()}");
    Console.WriteLine($"  {analyzer.AnalyzePressure()}");
    Console.WriteLine($"  {analyzer.AnalyzeSpeed()}");
    Console.WriteLine();
}
