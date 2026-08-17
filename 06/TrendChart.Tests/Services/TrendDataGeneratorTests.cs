using TrendChart.Core.Models;
using TrendChart.Core.Services;

namespace TrendChart.Tests.Services;

public sealed class TrendDataGeneratorTests
{
    [Fact]
    public void GenerateHistoricalData_ReturnsCorrectSeriesCount()
    {
        var generator = new TrendDataGenerator();
        var series = generator.GenerateHistoricalData(TimeSpan.FromHours(1), TimeSpan.FromMinutes(1));
        Assert.Equal(3, series.Count); // 温度、压力、转速
    }

    [Fact]
    public void GenerateHistoricalData_PointsCountMatchesDuration()
    {
        var generator = new TrendDataGenerator();
        var duration = TimeSpan.FromHours(1);
        var interval = TimeSpan.FromMinutes(1);
        var series = generator.GenerateHistoricalData(duration, interval);

        // 1 小时 / 1 分钟 = 61 个点（包含起始点）
        Assert.Equal(61, series[0].Points.Count);
    }

    [Fact]
    public void GenerateHistoricalData_SetsMinMaxCurrentValue()
    {
        var generator = new TrendDataGenerator();
        var series = generator.GenerateHistoricalData(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(1));

        foreach (var s in series)
        {
            Assert.True(s.MinValue <= s.MaxValue);
            Assert.True(s.MinValue <= s.CurrentValue);
            Assert.True(s.CurrentValue <= s.MaxValue);
        }
    }

    [Fact]
    public void GenerateHistoricalData_WithNegativeDuration_ThrowsArgumentOutOfRangeException()
    {
        var generator = new TrendDataGenerator();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            generator.GenerateHistoricalData(TimeSpan.FromMinutes(-1), TimeSpan.FromSeconds(1)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GenerateHistoricalData_WithNonPositiveInterval_ThrowsArgumentOutOfRangeException(long intervalTicks)
    {
        var generator = new TrendDataGenerator();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            generator.GenerateHistoricalData(TimeSpan.FromMinutes(1), TimeSpan.FromTicks(intervalTicks)));
    }

    [Fact]
    public void CalculateStatistics_ReturnsCorrectValues()
    {
        var generator = new TrendDataGenerator();
        var series = generator.GenerateHistoricalData(TimeSpan.FromHours(1), TimeSpan.FromMinutes(1));
        var stats = generator.CalculateStatistics(series[0].Points);

        Assert.True(stats.Count > 0);
        Assert.True(stats.Min <= stats.Average);
        Assert.True(stats.Average <= stats.Max);
        Assert.True(stats.StdDev >= 0);
    }

    [Fact]
    public void CalculateStatistics_WithOneTwoThree_ReturnsExactPopulationStatistics()
    {
        var generator = new TrendDataGenerator();
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var points = new[]
        {
            new TrendDataPoint(start, 1, "测试"),
            new TrendDataPoint(start.AddSeconds(1), 2, "测试"),
            new TrendDataPoint(start.AddSeconds(2), 3, "测试")
        };

        var stats = generator.CalculateStatistics(points);

        Assert.Equal(3, stats.Count);
        Assert.Equal(1, stats.Min);
        Assert.Equal(3, stats.Max);
        Assert.Equal(2, stats.Average);
        Assert.Equal(Math.Sqrt(2.0 / 3.0), stats.StdDev, precision: 12);
        Assert.Equal(points[0].Timestamp, stats.FirstTimestamp);
        Assert.Equal(points[^1].Timestamp, stats.LastTimestamp);
    }
}
