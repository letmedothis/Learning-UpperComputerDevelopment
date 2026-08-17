using ProductionLineMonitor.Core.Services;

namespace ProductionLineMonitor.Tests.Services;

public sealed class FakeDataGeneratorTests
{
    [Fact]
    public void Generate_CreatesValuesWithinSimulationRanges()
    {
        var generator = new FakeDataGenerator(new Random(20260817));
        var readings = Enumerable.Range(0, 100).Select(_ => generator.Generate()).ToArray();

        Assert.All(readings, reading =>
        {
            Assert.InRange(reading.Temperature, 5, 45);
            Assert.InRange(reading.Pressure, 0.01, 0.69);
            Assert.InRange(reading.Speed, 300, 2700);
        });
    }

    [Fact]
    public void Generate_IncreasesProductionAndResetStartsAgainFromZero()
    {
        var generator = new FakeDataGenerator(new Random(7));
        var first = generator.Generate();
        var second = generator.Generate();
        Assert.True(second.Production > first.Production);

        generator.Reset();
        var afterReset = generator.Generate();
        Assert.InRange(afterReset.Production, 1, 5);
    }

    [Fact]
    public void Generate_AssignsLevelsUsingReadingEvaluator()
    {
        var generator = new FakeDataGenerator(new Random(9));
        var reading = generator.Generate();
        Assert.Equal(ReadingEvaluator.EvaluateTemperature(reading.Temperature), reading.TemperatureLevel);
        Assert.Equal(ReadingEvaluator.EvaluatePressure(reading.Pressure), reading.PressureLevel);
        Assert.Equal(ReadingEvaluator.EvaluateSpeed(reading.Speed), reading.SpeedLevel);
    }
}
