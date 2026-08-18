namespace TrendChart.Core.Models;

/// <summary>
/// 趋势图系列（一条线）。
/// </summary>
public sealed class TrendSeries
{
    /// <summary>系列名称，如"温度"。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>计量单位，如"°C"。</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>线条颜色，十六进制格式。</summary>
    public string Color { get; set; } = "#4CAF50";

    /// <summary>数据点集合。</summary>
    public List<TrendDataPoint> Points { get; set; } = new();

    /// <summary>Y 轴最小值。</summary>
    public double MinValue { get; set; }

    /// <summary>Y 轴最大值。</summary>
    public double MaxValue { get; set; }

    /// <summary>当前最新值。</summary>
    public double CurrentValue { get; set; }
}
