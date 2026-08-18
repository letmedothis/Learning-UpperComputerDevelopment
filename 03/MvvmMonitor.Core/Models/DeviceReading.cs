namespace MvvmMonitor.Core.Models;

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
