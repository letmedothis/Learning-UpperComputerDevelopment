using TrendChart.Core.Models;

namespace TrendChart.Core.Services;

/// <summary>
/// 趋势图数据生成器 —— 生成模拟历史数据。
/// </summary>
public sealed class TrendDataGenerator
{
    private readonly Random _random = new();

    /// <summary>
    /// 生成指定时间范围内的模拟数据。
    /// </summary>
    public List<TrendSeries> GenerateHistoricalData(TimeSpan duration, TimeSpan interval)
    {
        // 负时间范围没有业务含义；非正间隔还会让时间游标无法向结束时间推进。
        if (duration < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration), "时间范围不能为负数");
        if (interval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(interval), "采样间隔必须大于零");

        var series = new List<TrendSeries>
        {
            new() { Name = "温度", Unit = "°C", Color = "#F44336" },
            new() { Name = "压力", Unit = "MPa", Color = "#2196F3" },
            new() { Name = "转速", Unit = "rpm", Color = "#4CAF50" }
        };

        var endTime = DateTime.Now;
        var startTime = endTime - duration;
        var currentTime = startTime;

        while (currentTime <= endTime)
        {
            series[0].Points.Add(new TrendDataPoint(currentTime, 20 + _random.NextDouble() * 20, "温度"));
            series[1].Points.Add(new TrendDataPoint(currentTime, 0.1 + _random.NextDouble() * 0.4, "压力"));
            series[2].Points.Add(new TrendDataPoint(currentTime, 800 + _random.NextDouble() * 1200, "转速"));

            currentTime += interval;
        }

        foreach (var s in series)
        {
            s.MinValue = s.Points.Min(p => p.Value);
            s.MaxValue = s.Points.Max(p => p.Value);
            s.CurrentValue = s.Points.Last().Value;
        }

        return series;
    }

    /// <summary>
    /// 计算统计数据。
    /// </summary>
    public TrendStatistics CalculateStatistics(IReadOnlyList<TrendDataPoint> points)
    {
        if (points.Count == 0)
            return new TrendStatistics();

        var values = points.Select(p => p.Value).ToList();
        return new TrendStatistics
        {
            Count = values.Count,
            Min = values.Min(),
            Max = values.Max(),
            Average = values.Average(),
            StdDev = CalculateStdDev(values),
            FirstTimestamp = points.First().Timestamp,
            LastTimestamp = points.Last().Timestamp
        };
    }

    private static double CalculateStdDev(IReadOnlyList<double> values)
    {
        var avg = values.Average();
        var sumSquares = values.Sum(v => (v - avg) * (v - avg));
        return Math.Sqrt(sumSquares / values.Count);
    }
}
