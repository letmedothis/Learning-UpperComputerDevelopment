namespace MvvmMonitor.Core.Models;

/// <summary>
/// 指标状态等级，按严重程度升序排列。
/// Java 对比：类似 enum，C# 的 enum 基于 int，可直接比较大小。
/// </summary>
public enum MetricLevel
{
    Normal = 0,
    Warning = 1,
    Alarm = 2
}
