namespace TrendChart.Core.Models;

/// <summary>
/// 趋势图数据点。
/// </summary>
public sealed record TrendDataPoint(
    DateTime Timestamp,
    double Value,
    string SeriesName);
