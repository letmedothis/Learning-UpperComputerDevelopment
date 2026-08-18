namespace MvvmMonitor.Core.Models;

/// <summary>
/// 一次不可变的设备采样数据。
/// Java 对比：类似 @Value 注解的 Lombok 类，C# 用 record 实现不可变 + 值相等。
/// </summary>
public sealed record DeviceReading(
    DateTimeOffset Timestamp,
    double Temperature,
    double Pressure,
    double Speed,
    int Production,
    MetricLevel TemperatureLevel,
    MetricLevel PressureLevel,
    MetricLevel SpeedLevel,
    MetricLevel OverallLevel);
