using MvvmMonitor.Core.Models;
using MvvmMonitor.Core.Services;

namespace MvvmMonitor.Tests.Services;

public sealed class ReadingEvaluatorTests
{
    [Theory]
    [InlineData(9.9, MetricLevel.Alarm)]
    [InlineData(10, MetricLevel.Warning)]
    [InlineData(15, MetricLevel.Normal)]
    [InlineData(35, MetricLevel.Normal)]
    [InlineData(40, MetricLevel.Warning)]
    [InlineData(40.1, MetricLevel.Alarm)]
    public void EvaluateTemperature_ReturnsExpectedLevel(double value, MetricLevel expected) =>
        Assert.Equal(expected, ReadingEvaluator.EvaluateTemperature(value));

    [Theory]
    [InlineData(0.049, MetricLevel.Alarm)]
    [InlineData(0.05, MetricLevel.Warning)]
    [InlineData(0.1, MetricLevel.Normal)]
    [InlineData(0.5, MetricLevel.Normal)]
    [InlineData(0.6, MetricLevel.Warning)]
    [InlineData(0.601, MetricLevel.Alarm)]
    public void EvaluatePressure_ReturnsExpectedLevel(double value, MetricLevel expected) =>
        Assert.Equal(expected, ReadingEvaluator.EvaluatePressure(value));

    [Theory]
    [InlineData(499, MetricLevel.Alarm)]
    [InlineData(500, MetricLevel.Warning)]
    [InlineData(1000, MetricLevel.Normal)]
    [InlineData(2000, MetricLevel.Normal)]
    [InlineData(2500, MetricLevel.Warning)]
    [InlineData(2501, MetricLevel.Alarm)]
    public void EvaluateSpeed_ReturnsExpectedLevel(double value, MetricLevel expected) =>
        Assert.Equal(expected, ReadingEvaluator.EvaluateSpeed(value));

    [Fact]
    public void GetOverallLevel_ReturnsMostSevereLevel() =>
        Assert.Equal(MetricLevel.Alarm, ReadingEvaluator.GetOverallLevel(MetricLevel.Normal, MetricLevel.Alarm, MetricLevel.Warning));
}
