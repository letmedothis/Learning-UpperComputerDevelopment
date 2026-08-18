namespace TrendChart.Core.Models;

/// <summary>
/// 趋势统计数据。
/// </summary>
public sealed class TrendStatistics
{
    /// <summary>数据点总数。</summary>
    public int Count { get; set; }

    /// <summary>最小值。</summary>
    public double Min { get; set; }

    /// <summary>最大值。</summary>
    public double Max { get; set; }

    /// <summary>平均值。</summary>
    public double Average { get; set; }

    /// <summary>标准差。</summary>
    public double StdDev { get; set; }

    /// <summary>最早时间戳。</summary>
    public DateTime FirstTimestamp { get; set; }

    /// <summary>最晚时间戳。</summary>
    public DateTime LastTimestamp { get; set; }

    /// <summary>时间跨度。</summary>
    public TimeSpan Duration => LastTimestamp - FirstTimestamp;
}
