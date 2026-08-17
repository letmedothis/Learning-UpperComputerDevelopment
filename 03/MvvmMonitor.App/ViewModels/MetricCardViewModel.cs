using MvvmMonitor.Core.Models;
using MvvmMonitor.Core.Mvvm;

namespace MvvmMonitor.App.ViewModels;

/// <summary>
/// 指标卡片 ViewModel，支持属性变更通知。
/// </summary>
public sealed class MetricCardViewModel : ViewModelBase
{
    private string _name = string.Empty;
    private string _value = "--";
    private string _unit = string.Empty;
    private string _statusText = "等待";
    private MetricLevel _level;

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Value { get => _value; set => SetProperty(ref _value, value); }
    public string Unit { get => _unit; set => SetProperty(ref _unit, value); }
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }
    public MetricLevel Level { get => _level; set => SetProperty(ref _level, value); }
}
