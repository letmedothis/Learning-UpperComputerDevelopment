using DeviceDataGenerator.Models;
using DeviceDataGenerator.Services;
using Xunit;

namespace DeviceDataGenerator.Tests;

public sealed class DataAnalyzerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenWindowSizeIsNotPositive_Throws(int maxSamples)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DataAnalyzer(maxSamples));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void GroupByTemperatureRange_WhenStepIsNotPositive_Throws(double step)
    {
        var analyzer = new DataAnalyzer();
        analyzer.AddReading(CreateReading(20));

        Assert.Throws<ArgumentOutOfRangeException>(() => analyzer.GroupByTemperatureRange(step).ToList());
    }

    [Fact]
    public void AddReading_WhenWindowIsFull_KeepsOnlyNewestReadings()
    {
        var analyzer = new DataAnalyzer(maxSamples: 2);

        analyzer.AddReading(CreateReading(10));
        analyzer.AddReading(CreateReading(20));
        analyzer.AddReading(CreateReading(30));

        Assert.Equal([20d, 30d], analyzer.GetReadings().Select(r => r.Temperature));
    }

    [Fact]
    public void AnalyzeTemperature_UsesExactValuesInCurrentWindow()
    {
        var analyzer = new DataAnalyzer();
        analyzer.AddReadings([CreateReading(10), CreateReading(20), CreateReading(30)]);

        var result = analyzer.AnalyzeTemperature();

        Assert.Equal(10, result.Min);
        Assert.Equal(30, result.Max);
        Assert.Equal(20, result.Average);
        Assert.Equal(3, result.Count);
    }

    private static DeviceReading CreateReading(double temperature) => new()
    {
        Timestamp = DateTime.UnixEpoch,
        Temperature = temperature,
        Pressure = 0.3,
        Speed = 1500
    };
}
