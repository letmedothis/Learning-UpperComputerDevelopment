using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.App.Models;

/// <summary>
/// 仪表盘的不可变快照，作为 DataContext 整体替换。
/// V1 不使用 INotifyPropertyChanged，每次采样替换整个对象。
/// </summary>
public sealed record DashboardState(
    string DeviceName,
    string OnlineStatus,
    OperatingState OperatingState,
    string OperatingStatus,
    bool CanStart,
    bool CanStop,
    bool CanReset,
    IReadOnlyList<MetricCardItem> Metrics,
    IReadOnlyList<DeviceReading> RecentReadings,
    string LastUpdatedText,
    string Message);
