using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.App.Models;

/// <summary>
/// 指标卡片的不可变绑定项。
/// </summary>
public sealed record MetricCardItem(
    string Name,
    string Value,
    string Unit,
    string StatusText,
    MetricLevel Level);
