namespace TrendChart.Core.Models;

/// <summary>
/// 趋势统计数据。
/// </summary>
public sealed class TrendStatistics
{
    public int Count { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
    public double StdDev { get; set; }
    public DateTime FirstTimestamp { get; set; }
    public DateTime LastTimestamp { get; set; }
    public TimeSpan Duration => LastTimestamp - FirstTimestamp;
}
