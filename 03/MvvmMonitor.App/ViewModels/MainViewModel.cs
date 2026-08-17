using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmMonitor.Core.Models;
using MvvmMonitor.Core.Mvvm;
using MvvmMonitor.App.Mvvm;
using MvvmMonitor.Core.Services;

namespace MvvmMonitor.App.ViewModels;

/// <summary>
/// 主窗口 ViewModel —— 管理所有 UI 状态和命令。
/// 使用 INotifyPropertyChanged 自动更新绑定，不再整体替换 DataContext。
/// </summary>
public sealed class MainViewModel : ViewModelBase, IDisposable
{
    private readonly FakeDataGenerator _generator = new();
    private readonly RecentReadingBuffer _buffer = new(20);
    private AcquisitionService? _acquisitionService;
    private CancellationTokenSource? _cts;
    private Task? _acquisitionTask;

    private string _deviceName = "CNC-001 数控机床";
    private string _onlineStatus = "● 在线";
    private string _operatingStatus = "■ 已停止";
    private string _lastUpdatedText = "最后更新: --";
    private string _message = "等待启动";
    private bool _canStart = true;
    private bool _canStop;
    private bool _canReset = true;
    private OperatingState _operatingState = OperatingState.Stopped;

    public MainViewModel()
    {
        StartCommand = new RelayCommand(ExecuteStart, () => CanStart);
        StopCommand = new RelayCommand(ExecuteStop, () => CanStop);
        ResetCommand = new RelayCommand(ExecuteReset, () => CanReset);

        // 初始化四个指标卡片
        TemperatureCard = new MetricCardViewModel { Name = "温度", Unit = "°C" };
        PressureCard = new MetricCardViewModel { Name = "压力", Unit = "MPa" };
        SpeedCard = new MetricCardViewModel { Name = "转速", Unit = "rpm" };
        ProductionCard = new MetricCardViewModel { Name = "产量", Unit = "件" };
    }

    // ── 属性 ──────────────────────────────────────────────

    public string DeviceName { get => _deviceName; set => SetProperty(ref _deviceName, value); }
    public string OnlineStatus { get => _onlineStatus; set => SetProperty(ref _onlineStatus, value); }
    public string OperatingStatus { get => _operatingStatus; set => SetProperty(ref _operatingStatus, value); }
    public string LastUpdatedText { get => _lastUpdatedText; set => SetProperty(ref _lastUpdatedText, value); }
    public string Message { get => _message; set => SetProperty(ref _message, value); }
    public bool CanStart { get => _canStart; set => SetProperty(ref _canStart, value); }
    public bool CanStop { get => _canStop; set => SetProperty(ref _canStop, value); }
    public bool CanReset { get => _canReset; set => SetProperty(ref _canReset, value); }

    public MetricCardViewModel TemperatureCard { get; }
    public MetricCardViewModel PressureCard { get; }
    public MetricCardViewModel SpeedCard { get; }
    public MetricCardViewModel ProductionCard { get; }

    public ObservableCollection<DeviceReading> RecentReadings { get; } = new();

    // ── 命令 ──────────────────────────────────────────────

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ResetCommand { get; }

    private async void ExecuteStart()
    {
        if (_operatingState != OperatingState.Stopped) return;

        _operatingState = OperatingState.Running;
        UpdateButtonStates();
        _cts = new CancellationTokenSource();
        _acquisitionService = new AcquisitionService(_generator);
        Message = "采集中...";

        _acquisitionTask = RunAcquisitionAsync(_cts);
        await _acquisitionTask;
    }

    private async void ExecuteStop()
    {
        if (_operatingState != OperatingState.Running) return;
        await StopAcquisitionAsync();
    }

    private void ExecuteReset()
    {
        if (_operatingState != OperatingState.Stopped) return;

        _generator.Reset();
        _buffer.Clear();
        RecentReadings.Clear();

        TemperatureCard.Value = "--";
        TemperatureCard.StatusText = "等待";
        TemperatureCard.Level = MetricLevel.Normal;

        PressureCard.Value = "--";
        PressureCard.StatusText = "等待";
        PressureCard.Level = MetricLevel.Normal;

        SpeedCard.Value = "--";
        SpeedCard.StatusText = "等待";
        SpeedCard.Level = MetricLevel.Normal;

        ProductionCard.Value = "0";
        ProductionCard.StatusText = "等待";
        ProductionCard.Level = MetricLevel.Normal;

        LastUpdatedText = "最后更新: --";
        Message = "已复位";
    }

    // ── 采集逻辑 ──────────────────────────────────────────

    private async Task RunAcquisitionAsync(CancellationTokenSource runCts)
    {
        Exception? failure = null;

        try
        {
            await _acquisitionService!.RunAsync(
                reading => ApplyReadingAsync(reading),
                runCts.Token);
        }
        catch (OperationCanceledException) when (runCts.IsCancellationRequested)
        {
            // 正常取消
        }
        catch (Exception ex)
        {
            failure = ex;
        }
        finally
        {
            runCts.Dispose();
            if (_cts == runCts) _cts = null;
            _operatingState = OperatingState.Stopped;
            UpdateButtonStates();
            // 正常取消可以显示“已停止”；异常完成必须保留故障，避免 finally 掩盖根因。
            Message = AcquisitionUiState.GetCompletionMessage(failure);
        }
    }

    private async Task StopAcquisitionAsync()
    {
        if (_cts == null || _acquisitionTask == null) return;

        _operatingState = OperatingState.Stopping;
        UpdateButtonStates();
        Message = "正在停止...";
        _cts.Cancel();

        try
        {
            await _acquisitionTask;
        }
        catch (OperationCanceledException)
        {
            // 预期
        }

        _acquisitionTask = null;
    }

    private Task ApplyReadingAsync(DeviceReading reading)
    {
        return System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _buffer.Add(reading);

            // 更新指标卡片（属性变更自动通知 UI）
            TemperatureCard.Value = reading.Temperature.ToString("F1");
            TemperatureCard.StatusText = GetLevelText(reading.TemperatureLevel);
            TemperatureCard.Level = reading.TemperatureLevel;

            PressureCard.Value = reading.Pressure.ToString("F3");
            PressureCard.StatusText = GetLevelText(reading.PressureLevel);
            PressureCard.Level = reading.PressureLevel;

            SpeedCard.Value = reading.Speed.ToString("F1");
            SpeedCard.StatusText = GetLevelText(reading.SpeedLevel);
            SpeedCard.Level = reading.SpeedLevel;

            ProductionCard.Value = reading.Production.ToString();
            ProductionCard.StatusText = "累计";
            ProductionCard.Level = MetricLevel.Normal;

            // 更新最近读数集合
            RecentReadings.Clear();
            foreach (var item in _buffer.Snapshot)
                RecentReadings.Add(item);

            LastUpdatedText = $"最后更新: {reading.Timestamp:HH:mm:ss}";
            Message = "采集中...";
        }).Task;
    }

    private void UpdateButtonStates()
    {
        OperatingStatus = AcquisitionUiState.GetOperatingStatus(_operatingState);
        CanStart = _operatingState == OperatingState.Stopped;
        CanStop = _operatingState == OperatingState.Running;
        CanReset = _operatingState == OperatingState.Stopped;
    }

    private static string GetLevelText(MetricLevel level) => level switch
    {
        MetricLevel.Normal => "正常",
        MetricLevel.Warning => "警告",
        MetricLevel.Alarm => "报警",
        _ => "未知"
    };

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
