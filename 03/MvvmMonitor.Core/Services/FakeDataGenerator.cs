using MvvmMonitor.Core.Models;

namespace MvvmMonitor.Core.Services;

public sealed class FakeDataGenerator
{
    private readonly Random _random;
    private int _production;

    public FakeDataGenerator(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public int Production => _production;

    public DeviceReading Generate()
    {
        var temperature = Math.Round(5 + _random.NextDouble() * 40, 1);
        var pressure = Math.Round(0.01 + _random.NextDouble() * 0.68, 3);
        var speed = Math.Round(300 + _random.NextDouble() * 2400, 1);

        _production += _random.Next(1, 6);

        var tempLevel = ReadingEvaluator.EvaluateTemperature(temperature);
        var pressLevel = ReadingEvaluator.EvaluatePressure(pressure);
        var speedLevel = ReadingEvaluator.EvaluateSpeed(speed);
        var overall = ReadingEvaluator.GetOverallLevel(tempLevel, pressLevel, speedLevel);

        return new DeviceReading(
            DateTime.Now, temperature, pressure, speed, _production,
            tempLevel, pressLevel, speedLevel, overall);
    }

    public void Reset() => _production = 0;
}
