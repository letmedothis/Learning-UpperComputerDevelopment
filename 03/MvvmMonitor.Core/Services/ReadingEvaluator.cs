using MvvmMonitor.Core.Models;

namespace MvvmMonitor.Core.Services;

public static class ReadingEvaluator
{
    public static MetricLevel EvaluateTemperature(double value)
    {
        if (!double.IsFinite(value)) return MetricLevel.Alarm;
        if (value >= 15 && value <= 35) return MetricLevel.Normal;
        if (value >= 10 && value <= 40) return MetricLevel.Warning;
        return MetricLevel.Alarm;
    }

    public static MetricLevel EvaluatePressure(double value)
    {
        if (!double.IsFinite(value)) return MetricLevel.Alarm;
        if (value >= 0.1 && value <= 0.5) return MetricLevel.Normal;
        if (value >= 0.05 && value <= 0.6) return MetricLevel.Warning;
        return MetricLevel.Alarm;
    }

    public static MetricLevel EvaluateSpeed(double value)
    {
        if (!double.IsFinite(value)) return MetricLevel.Alarm;
        if (value >= 1000 && value <= 2000) return MetricLevel.Normal;
        if (value >= 500 && value <= 2500) return MetricLevel.Warning;
        return MetricLevel.Alarm;
    }

    public static MetricLevel GetOverallLevel(params MetricLevel[] levels)
    {
        if (levels.Length == 0)
            throw new ArgumentException("至少需要一个指标等级", nameof(levels));
        return levels.Max();
    }
}
