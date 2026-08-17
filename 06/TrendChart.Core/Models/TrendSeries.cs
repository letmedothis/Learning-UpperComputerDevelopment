namespace TrendChart.Core.Models;

/// <summary>
/// 趋势图系列（一条线）。
/// </summary>
public sealed class TrendSeries
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Color { get; set; } = "#4CAF50";
    public List<TrendDataPoint> Points { get; set; } = new();
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double CurrentValue { get; set; }
}
