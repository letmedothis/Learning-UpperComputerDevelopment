using System.ComponentModel;
using System.Windows;
using ProductionLineMonitor.App.Models;
using ProductionLineMonitor.Core.Models;
using ProductionLineMonitor.Core.Services;

namespace ProductionLineMonitor.App;

/// <summary>
/// 主窗口 —— V1 采用"替换绑定快照"方式，不使用 INotifyPropertyChanged。
/// Java 对比：类似 Swing 的 EDT 线程切换，C# 用 Dispatcher.InvokeAsync。
/// </summary>
public partial class MainWindow : Window
{
    private readonly FakeDataGenerator _generator = new();
    private readonly RecentReadingBuffer _buffer = new(20);
    private AcquisitionService? _acquisitionService;
    private CancellationTokenSource? _cts;
    private Task? _acquisitionTask;

    private OperatingState _operatingState = OperatingState.Stopped;
    private DeviceReading? _latestReading;
    private string _message = "等待启动";
    private bool _isClosing;
    private bool _shutdownCompleted;

    public MainWindow()
    {
        InitializeComponent();
        RefreshDashboard("等待启动");
    }

    // ── 按钮事件 ──────────────────────────────────────────

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (_operatingState != OperatingState.Stopped) return;

        _operatingState = OperatingState.Running;
        _cts = new CancellationTokenSource();
        _acquisitionService = new AcquisitionService(_generator);
        RefreshDashboard("采集中...");

        _acquisitionTask = RunAcquisitionAsync(_cts);
        await _acquisitionTask;
    }

    private async void StopButton_Click(object sender, RoutedEventArgs e)
    {
        if (_operatingState != OperatingState.Running) return;
        await StopAcquisitionAsync();
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        if (_operatingState != OperatingState.Stopped) return;

        _generator.Reset();
        _buffer.Clear();
        _latestReading = null;
        RefreshDashboard("已复位");
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
            // 正常取消，不显示错误
        }
        catch (Exception ex)
        {
            failure = ex;
        }
        finally
        {
            runCts.Dispose();
            if (_cts == runCts) _cts = null;
            if (!_isClosing)
            {
                _operatingState = OperatingState.Stopped;
                // 正常取消可以显示“已停止”；异常完成必须保留故障，避免 finally 掩盖根因。
                _message = AcquisitionUiState.GetCompletionMessage(failure);
                RefreshDashboard(_message);
            }
        }
    }

    private async Task StopAcquisitionAsync()
    {
        if (_cts == null || _acquisitionTask == null) return;

        _operatingState = OperatingState.Stopping;
        // 关闭窗口时由 Window_Closing 设置更详细的消息，避免覆盖
        if (!_isClosing)
            RefreshDashboard("正在停止...");
        _cts.Cancel();

        try
        {
            await _acquisitionTask;
        }
        catch (OperationCanceledException)
        {
            // 预期的取消异常
        }

        _acquisitionTask = null;
    }

    private Task ApplyReadingAsync(DeviceReading reading)
    {
        return Dispatcher.InvokeAsync(() =>
        {
            Dispatcher.VerifyAccess();
            if (_isClosing) return;

            _buffer.Add(reading);
            _latestReading = reading;
            RefreshDashboard("采集中...");
        }).Task;
    }

    // ── UI 刷新 ──────────────────────────────────────────

    private void RefreshDashboard(string message)
    {
        Dispatcher.VerifyAccess();

        var metrics = new List<MetricCardItem>();
        if (_latestReading != null)
        {
            metrics.Add(new MetricCardItem("温度", _latestReading.Temperature.ToString("F1"), "°C",
                GetLevelText(_latestReading.TemperatureLevel), _latestReading.TemperatureLevel));
            metrics.Add(new MetricCardItem("压力", _latestReading.Pressure.ToString("F3"), "MPa",
                GetLevelText(_latestReading.PressureLevel), _latestReading.PressureLevel));
            metrics.Add(new MetricCardItem("转速", _latestReading.Speed.ToString("F1"), "rpm",
                GetLevelText(_latestReading.SpeedLevel), _latestReading.SpeedLevel));
            metrics.Add(new MetricCardItem("产量", _latestReading.Production.ToString(), "件",
                "累计", MetricLevel.Normal));
        }
        else
        {
            metrics.Add(new MetricCardItem("温度", "--", "°C", "等待", MetricLevel.Normal));
            metrics.Add(new MetricCardItem("压力", "--", "MPa", "等待", MetricLevel.Normal));
            metrics.Add(new MetricCardItem("转速", "--", "rpm", "等待", MetricLevel.Normal));
            metrics.Add(new MetricCardItem("产量", "0", "件", "等待", MetricLevel.Normal));
        }

        DataContext = new DashboardState(
            DeviceName: "CNC-001 数控机床",
            OnlineStatus: "● 在线",
            OperatingState: _operatingState,
            OperatingStatus: AcquisitionUiState.GetOperatingStatus(_operatingState),
            CanStart: _operatingState == OperatingState.Stopped,
            CanStop: _operatingState == OperatingState.Running,
            CanReset: _operatingState == OperatingState.Stopped,
            Metrics: metrics,
            RecentReadings: _buffer.Snapshot,
            LastUpdatedText: _latestReading != null
                ? $"最后更新: {_latestReading.Timestamp:HH:mm:ss}"
                : "最后更新: --",
            Message: message);
    }

    private static string GetLevelText(MetricLevel level) => level switch
    {
        MetricLevel.Normal => "正常",
        MetricLevel.Warning => "警告",
        MetricLevel.Alarm => "报警",
        _ => "未知"
    };

    // ── 窗口关闭 ──────────────────────────────────────────

    private async void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (_shutdownCompleted) return;

        e.Cancel = true;
        _isClosing = true;
        _operatingState = OperatingState.Stopping;
        RefreshDashboard("正在停止采集并关闭窗口...");
        await StopAcquisitionAsync();
        _shutdownCompleted = true;
        Close();
    }
}
