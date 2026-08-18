using MvvmMonitor.Core.Models;

namespace MvvmMonitor.Core.Services;

/// <summary>
/// 指标阈值判断器 —— 纯函数，无状态。
/// 判断顺序：先 Normal 闭区间，再 Warning 闭区间，否则 Alarm。
/// </summary>
public static class ReadingEvaluator
{
    /// <summary>评估温度指标等级。</summary>
    public static MetricLevel EvaluateTemperature(double value)
    {
        if (!double.IsFinite(value)) return MetricLevel.Alarm;
        if (value >= 15 && value <= 35) return MetricLevel.Normal;
        if (value >= 10 && value <= 40) return MetricLevel.Warning;
        return MetricLevel.Alarm;
    }

    /// <summary>评估压力指标等级。</summary>
    public static MetricLevel EvaluatePressure(double value)
    {
        if (!double.IsFinite(value)) return MetricLevel.Alarm;
        if (value >= 0.1 && value <= 0.5) return MetricLevel.Normal;
        if (value >= 0.05 && value <= 0.6) return MetricLevel.Warning;
        return MetricLevel.Alarm;
    }

    /// <summary>评估转速指标等级。</summary>
    public static MetricLevel EvaluateSpeed(double value)
    {
        if (!double.IsFinite(value)) return MetricLevel.Alarm;
        if (value >= 1000 && value <= 2000) return MetricLevel.Normal;
        if (value >= 500 && value <= 2500) return MetricLevel.Warning;
        return MetricLevel.Alarm;
    }

    /// <summary>
    /// 返回多个等级中的最高严重程度。
    /// </summary>
    public static MetricLevel GetOverallLevel(params MetricLevel[] levels)
    {
        if (levels.Length == 0)
            throw new ArgumentException("至少需要一个指标等级", nameof(levels));
        return levels.Max();
    }
}
