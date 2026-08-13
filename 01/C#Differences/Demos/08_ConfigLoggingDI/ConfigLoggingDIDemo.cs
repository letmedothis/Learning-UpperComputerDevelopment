using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace C_Differences.Demos._08_ConfigLoggingDI;

/// <summary>
/// 配置、日志、依赖注入 演示
///
/// Java 对比：
/// - 配置: Java properties/yaml → C# appsettings.json + IConfiguration
/// - 日志: Log4j/SLF4J → C# ILogger (内置)
/// - DI:  Spring IoC → C# Microsoft.Extensions.DependencyInjection (内置)
///
/// 关键区别：
/// - C# 的配置/日志/DI 是框架内置的，不需要第三方库
/// - 通过 Microsoft.Extensions.* 命名空间提供
/// </summary>
public class ConfigLoggingDIDemo
{
    public void Demo()
    {
        Console.WriteLine("1. 配置系统 (IConfiguration):");
        ConfigurationDemo();

        Console.WriteLine("\n2. 日志系统 (ILogger):");
        LoggingDemo();

        Console.WriteLine("\n3. 依赖注入 (DI):");
        DependencyInjectionDemo();

        Console.WriteLine("\n4. 综合应用 - 使用 DI 的设备监控服务:");
        PracticalExample();
    }

    /// <summary>
    /// 配置系统演示
    /// </summary>
    private void ConfigurationDemo()
    {
        // 创建内存配置（实际项目通常从 appsettings.json 读取）
        var configData = new Dictionary<string, string?>
        {
            ["AppSettings:AppName"] = "设备监控系统",
            ["AppSettings:Version"] = "1.0.0",
            ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=Sensors",
            ["DeviceSettings:TemperatureThreshold"] = "30",
            ["DeviceSettings:SamplingInterval"] = "1000"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // 读取配置
        string appName = configuration["AppSettings:AppName"] ?? "未知应用";
        string version = configuration["AppSettings:Version"] ?? "0.0.0";
        string? connString = configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"   应用名称: {appName}");
        Console.WriteLine($"   版本: {version}");
        Console.WriteLine($"   连接字符串: {connString}");

        // 强类型配置（推荐方式）
        var appSettings = new AppSettings();
        configuration.GetSection("AppSettings").Bind(appSettings);
        Console.WriteLine($"   强类型配置: {appSettings.AppName} v{appSettings.Version}");

        // 读取嵌套配置
        int threshold = int.Parse(configuration["DeviceSettings:TemperatureThreshold"] ?? "0");
        Console.WriteLine($"   温度阈值: {threshold}°C");
    }

    /// <summary>
    /// 日志系统演示
    /// </summary>
    private void LoggingDemo()
    {
        // 创建 LoggerFactory
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug);
        });

        // 创建 Logger
        var logger = loggerFactory.CreateLogger<ConfigLoggingDIDemo>();

        // 不同日志级别
        logger.LogDebug("这是一条调试日志");
        logger.LogInformation("这是一条信息日志");
        logger.LogWarning("这是一条警告日志");
        logger.LogError("这是一条错误日志");

        // 带参数的日志（结构化日志）
        string sensorName = "温度传感器";
        double value = 25.5;
        logger.LogInformation("传感器 {SensorName} 读数: {Value}°C", sensorName, value);

        // 带异常的日志
        try
        {
            throw new InvalidOperationException("模拟的异常");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "处理传感器数据时发生错误");
        }
    }

    /// <summary>
    /// 依赖注入演示
    /// </summary>
    private void DependencyInjectionDemo()
    {
        // 创建 DI 容器
        var services = new ServiceCollection();

        // 注册服务
        // AddTransient: 每次请求创建新实例
        services.AddTransient<ISensorService, TemperatureSensorService>();

        // AddSingleton: 整个应用生命周期只有一个实例
        services.AddSingleton<IDeviceRepository, InMemoryDeviceRepository>();

        // AddScoped: 每个请求作用域一个实例（Web 应用中常用）
        services.AddScoped<IDataProcessor, DataProcessor>();

        // 构建 ServiceProvider
        var serviceProvider = services.BuildServiceProvider();

        // 使用服务
        var sensorService = serviceProvider.GetRequiredService<ISensorService>();
        var repository = serviceProvider.GetRequiredService<IDeviceRepository>();

        var reading = sensorService.ReadSensor("温度传感器");
        repository.SaveReading(reading);

        Console.WriteLine($"   传感器: {reading.SensorName}");
        Console.WriteLine($"   值: {reading.Value:F1}°C");
        Console.WriteLine($"   时间: {reading.Timestamp:HH:mm:ss}");

        var history = repository.GetRecentReadings(1);
        Console.WriteLine($"   仓库中保存了 {history.Count()} 条记录");
    }

    /// <summary>
    /// 综合应用：使用 DI 的设备监控服务
    /// </summary>
    private void PracticalExample()
    {
        // 创建配置
        var configData = new Dictionary<string, string?>
        {
            ["MonitoringSettings:AlertThreshold"] = "30",
            ["MonitoringSettings:CheckInterval"] = "500"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // 创建 DI 容器
        var services = new ServiceCollection();

        // 注册配置
        services.AddSingleton<IConfiguration>(configuration);

        // 注册日志
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // 注册业务服务
        services.AddSingleton<ISensorService, TemperatureSensorService>();
        services.AddSingleton<IMonitoringService, MonitoringService>();

        var serviceProvider = services.BuildServiceProvider();

        // 使用监控服务
        var monitoringService = serviceProvider.GetRequiredService<IMonitoringService>();
        monitoringService.StartMonitoring();

        // 模拟运行一段时间
        Thread.Sleep(1500);

        monitoringService.StopMonitoring();
    }
}

// ========== 配置模型 ==========

public class AppSettings
{
    public string AppName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

// ========== 服务接口 ==========

public interface ISensorService
{
    SensorReading ReadSensor(string sensorName);
}

public interface IDeviceRepository
{
    void SaveReading(SensorReading reading);
    IEnumerable<SensorReading> GetRecentReadings(int count);
}

public interface IDataProcessor
{
    SensorReading Process(SensorReading rawReading);
}

public interface IMonitoringService
{
    void StartMonitoring();
    void StopMonitoring();
}

// ========== 服务实现 ==========

public class TemperatureSensorService : ISensorService
{
    private readonly Random _random = new();

    public SensorReading ReadSensor(string sensorName)
    {
        // 模拟传感器读取
        return new SensorReading
        {
            SensorName = sensorName,
            Value = 20 + _random.NextDouble() * 15,
            Timestamp = DateTime.Now
        };
    }
}

public class InMemoryDeviceRepository : IDeviceRepository
{
    private readonly List<SensorReading> _readings = new();

    public void SaveReading(SensorReading reading)
    {
        _readings.Add(reading);
    }

    public IEnumerable<SensorReading> GetRecentReadings(int count)
    {
        return _readings.TakeLast(count);
    }
}

public class DataProcessor : IDataProcessor
{
    public SensorReading Process(SensorReading rawReading)
    {
        // 简单的数据处理：校准
        return rawReading with { Value = rawReading.Value * 0.95 + 2.5 };
    }
}

public class MonitoringService : IMonitoringService
{
    private readonly ISensorService _sensorService;
    private readonly ILogger<MonitoringService> _logger;
    private readonly IConfiguration _configuration;
    private bool _isRunning;
    private CancellationTokenSource? _cts;

    public MonitoringService(
        ISensorService sensorService,
        ILogger<MonitoringService> logger,
        IConfiguration configuration)
    {
        _sensorService = sensorService;
        _logger = logger;
        _configuration = configuration;
    }

    public void StartMonitoring()
    {
        if (_isRunning) return;

        _isRunning = true;
        _cts = new CancellationTokenSource();

        _logger.LogInformation("监控服务启动");

        // 启动后台监控任务
        Task.Run(() => MonitorLoop(_cts.Token));

        _isRunning = true;
    }

    public void StopMonitoring()
    {
        if (!_isRunning) return;

        _cts?.Cancel();
        _isRunning = false;
        _logger.LogInformation("监控服务停止");
    }

    private async Task MonitorLoop(CancellationToken cancellationToken)
    {
        double threshold = double.Parse(
            _configuration["MonitoringSettings:AlertThreshold"] ?? "30");
        int interval = int.Parse(
            _configuration["MonitoringSettings:CheckInterval"] ?? "1000");

        while (!cancellationToken.IsCancellationRequested)
        {
            var reading = _sensorService.ReadSensor("温度传感器");

            _logger.LogDebug("传感器读数: {Value:F1}°C", reading.Value);

            if (reading.Value > threshold)
            {
                _logger.LogWarning("⚠️ 温度超限: {Value:F1}°C > {Threshold}°C",
                    reading.Value, threshold);
            }

            try
            {
                await Task.Delay(interval, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}

// ========== 数据模型 ==========

public record SensorReading
{
    public string SensorName { get; init; } = string.Empty;
    public double Value { get; init; }
    public DateTime Timestamp { get; init; }
}
