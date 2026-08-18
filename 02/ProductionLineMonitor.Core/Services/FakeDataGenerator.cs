using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.Core.Services;

/// <summary>
/// 生成一条模拟设备采样数据。
/// 不负责循环或线程，只负责生成一条数据，便于单元测试。
/// </summary>
public sealed class FakeDataGenerator
{
    private readonly Random _random;
    private int _production;

    public FakeDataGenerator(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public int Production => _production;

    /// <summary>
    /// 生成一条模拟采样，覆盖正常/警告/报警范围。
    /// </summary>
    public DeviceReading Generate()
    {
        var temperature = Math.Round(5 + _random.NextDouble() * 40, 1);   // [5, 45)
        var pressure = Math.Round(0.01 + _random.NextDouble() * 0.68, 3); // [0.01, 0.69)
        var speed = Math.Round(300 + _random.NextDouble() * 2400, 1);     // [300, 2700)

        _production += _random.Next(1, 6); // 每次增加 1~5

        var tempLevel = ReadingEvaluator.EvaluateTemperature(temperature);
        var pressLevel = ReadingEvaluator.EvaluatePressure(pressure);
        var speedLevel = ReadingEvaluator.EvaluateSpeed(speed);
        var overall = ReadingEvaluator.GetOverallLevel(tempLevel, pressLevel, speedLevel);

        return new DeviceReading(
            DateTimeOffset.UtcNow, temperature, pressure, speed, _production,
            tempLevel, pressLevel, speedLevel, overall);
    }

    /// <summary>
    /// 重置累计产量为零。
    /// </summary>
    public void Reset() => _production = 0;
}
