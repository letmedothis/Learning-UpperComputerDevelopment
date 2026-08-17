using DeviceDataGenerator.Models;
using Microsoft.Extensions.Logging;

namespace DeviceDataGenerator.Services;

/// <summary>
/// 设备数据生成器服务
///
/// 【综合应用的知识点】
/// 1. 事件（event）: 发布新数据，通知订阅者
/// 2. async/await: 异步生成数据，不阻塞主线程
/// 3. CancellationToken: 支持优雅取消，不强制终止线程
/// 4. IDisposable: 释放 CancellationTokenSource 资源
/// 5. 依赖注入: 通过构造函数注入 ILogger 和 SensorConfig
///
/// 【使用示例】
/// <code>
/// using var generator = new DeviceDataGeneratorService(config, logger);
/// generator.DataReceived += (sender, reading) => Console.WriteLine(reading);
///
/// using var cts = new CancellationTokenSource();
/// cts.CancelAfter(TimeSpan.FromSeconds(10)); // 10秒后自动停止
///
/// await generator.StartAsync(cts.Token);
/// </code>
/// </summary>
public class DeviceDataGeneratorService : IDisposable
{
    // ========== 私有字段 ==========

    /// <summary>传感器配置 - 决定生成数据的基准值和波动范围</summary>
    private readonly SensorConfig _config;

    /// <summary>日志记录器 - 用于记录运行状态和调试信息</summary>
    private readonly ILogger<DeviceDataGeneratorService> _logger;

    /// <summary>随机数生成器 - 用于模拟传感器数据波动</summary>
    private readonly Random _random = new();

    /// <summary>保护运行状态与 CTS 的发布/回收，确保同一时间最多只有一个采集循环。</summary>
    private readonly object _stateLock = new();

    /// <summary>
    /// 取消令牌源 - 用于控制数据生成循环的停止
    /// 注意：这是内部的 CancellationTokenSource，可以与外部的 CancellationToken 链接
    /// </summary>
    private CancellationTokenSource? _cts;

    /// <summary>运行状态标志</summary>
    private bool _isRunning;

    /// <summary>释放状态标志 - 防止重复释放</summary>
    private bool _disposed;

    // ========== 事件定义 ==========

    /// <summary>
    /// 新数据到达事件
    ///
    /// 【事件使用模式】
    /// 发布者（Generator）定义事件，订阅者（外部代码）注册回调
    /// 当有新数据时，发布者触发事件，所有订阅者的回调会被调用
    ///
    /// <code>
    /// // 订阅事件
    /// generator.DataReceived += (sender, reading) =>
    /// {
    ///     Console.WriteLine($"收到数据: {reading}");
    /// };
    /// </code>
    /// </summary>
    public event EventHandler<DeviceReading>? DataReceived;

    /// <summary>
    /// 数据采集状态变化事件
    /// 用于通知外部代码采集的启动和停止
    /// </summary>
    public event EventHandler<string>? StatusChanged;

    // ========== 构造函数 ==========

    /// <summary>
    /// 构造函数 - 依赖注入
    ///
    /// 【依赖注入模式】
    /// 通过构造函数注入依赖，而不是在类内部创建
    /// 这样可以方便地替换实现（如测试时使用 Mock）
    ///
    /// <code>
    /// // DI 容器会自动注入
    /// services.AddSingleton&lt;DeviceDataGeneratorService&gt;();
    /// </code>
    /// </summary>
    /// <param name="config">传感器配置</param>
    /// <param name="logger">日志记录器</param>
    public DeviceDataGeneratorService(SensorConfig config, ILogger<DeviceDataGeneratorService> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ========== 公共属性 ==========

    /// <summary>
    /// 是否正在运行
    /// 使用表达式体语法 =&gt; 简化只读属性
    /// </summary>
    public bool IsRunning
    {
        get
        {
            lock (_stateLock)
                return _isRunning;
        }
    }

    // ========== 公共方法 ==========

    /// <summary>
    /// 启动数据采集
    ///
    /// 【异步方法模式】
    /// 1. 返回 Task 表示异步操作
    /// 2. 接受 CancellationToken 参数支持取消
    /// 3. 使用 async/await 实现异步逻辑
    ///
    /// <code>
    /// // 启动采集，5秒后取消
    /// using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    /// await generator.StartAsync(cts.Token);
    /// </code>
    /// </summary>
    /// <param name="cancellationToken">
    /// 外部取消令牌（可选）
    /// 可以链接到 UI 的取消按钮或超时取消
    /// </param>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        CancellationTokenSource runCts;

        // “检查是否运行”和“登记本轮 CTS”必须是同一个原子步骤，
        // 否则并发调用可能同时通过检查，产生 Stop 无法全部取消的后台循环。
        lock (_stateLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_isRunning)
            {
                _logger.LogWarning("数据生成器已在运行");
                return;
            }

            runCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _cts = runCts;
            _isRunning = true;
        }

        try
        {
            // 状态事件也放在 try 内：订阅者失败时仍能执行 finally，释放本轮运行权。
            OnStatusChanged("数据采集启动");
            _logger.LogInformation("数据生成器启动，采样间隔: 1秒");

            // 进入数据生成主循环
            await GenerateDataAsync(runCts.Token);
        }
        catch (OperationCanceledException) when (runCts.IsCancellationRequested)
        {
            // 只吞掉由本轮令牌发起的正常取消；订阅者自己的取消异常仍应暴露。
            _logger.LogInformation("数据生成器被取消");
        }
        finally
        {
            try
            {
                // 先发布停止事件，再释放“单运行”门闩，保证观察者不会看到新启动先于旧停止。
                OnStatusChanged("数据采集停止");
            }
            finally
            {
                lock (_stateLock)
                {
                    if (ReferenceEquals(_cts, runCts))
                    {
                        _cts = null;
                        _isRunning = false;
                    }
                }

                // StartAsync 创建并拥有 CTS；必须等循环结束后再释放，Dispose 只发取消信号。
                runCts.Dispose();
            }
        }
    }

    /// <summary>
    /// 停止数据采集
    ///
    /// 【协作式取消】
    /// 不是强制终止线程，而是设置取消标志
    /// 数据生成循环会在下一次检查时发现取消请求并优雅退出
    /// </summary>
    public void Stop()
    {
        lock (_stateLock)
        {
            if (!_isRunning) return;

            _logger.LogInformation("正在停止数据生成器...");
            // 在同一锁内取消，避免 StartAsync 的 finally 同时释放 CTS。
            _cts?.Cancel();
        }
    }

    // ========== 私有方法 ==========

    /// <summary>
    /// 生成数据的主循环
    ///
    /// 【循环中的取消检查】
    /// 1. while 条件检查 IsCancellationRequested
    /// 2. Task.Delay 也接受 CancellationToken
    /// 3. 任何一步被取消都会抛出 OperationCanceledException
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task GenerateDataAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // 生成一条模拟数据
            var reading = GenerateReading();

            // 触发事件，通知所有订阅者
            OnDataReceived(reading);

            _logger.LogDebug("生成数据: {Reading}", reading);

            try
            {
                // 等待 1 秒，同时支持取消
                // Task.Delay 会监听 CancellationToken，取消时抛出异常
                await Task.Delay(1000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Task.Delay 被取消时会抛出异常
                // 记录日志后重新抛出，让上层处理
                _logger.LogInformation("数据生成循环被取消");
                throw;
            }
        }
    }

    /// <summary>
    /// 生成单条读数
    ///
    /// 【算法说明】
    /// 实际值 = 基准值 + (随机数 - 0.5) × 波动范围
    /// 其中 (随机数 - 0.5) 的范围是 [-0.5, 0.5]
    /// 所以实际值的范围是 [基准值 - 波动范围/2, 基准值 + 波动范围/2]
    /// </summary>
    /// <returns>生成的设备读数</returns>
    private DeviceReading GenerateReading()
    {
        return new DeviceReading
        {
            Timestamp = DateTime.Now,
            // 温度: 25 ± 5°C
            Temperature = _config.TemperatureBase + (_random.NextDouble() - 0.5) * _config.TemperatureRange,
            // 压力: 0.3 ± 0.1 MPa
            Pressure = _config.PressureBase + (_random.NextDouble() - 0.5) * _config.PressureRange,
            // 转速: 1500 ± 250 rpm
            Speed = _config.SpeedBase + (_random.NextDouble() - 0.5) * _config.SpeedRange
        };
    }

    /// <summary>
    /// 触发数据接收事件
    ///
    /// 【事件触发模式】
    /// 1. 使用 ?.Invoke 安全调用（如果没有任何订阅者则不调用）
    /// 2. 传递 this 作为 sender，传递数据作为 EventArgs
    /// 3. 使用 virtual 允许子类重写事件触发逻辑
    /// </summary>
    /// <param name="reading">新生成的读数</param>
    protected virtual void OnDataReceived(DeviceReading reading)
    {
        DataReceived?.Invoke(this, reading);
    }

    /// <summary>
    /// 触发状态变化事件
    /// </summary>
    /// <param name="status">状态描述</param>
    protected virtual void OnStatusChanged(string status)
    {
        StatusChanged?.Invoke(this, status);
    }

    // ========== IDisposable 实现 ==========

    /// <summary>
    /// 释放资源 - IDisposable 接口实现
    ///
    /// 【IDisposable 模式】
    /// 1. 释放非托管资源（这里主要是 CancellationTokenSource）
    /// 2. 使用 _disposed 标志防止重复释放
    /// 3. 与 using 语句配合使用，确保资源被释放
    ///
    /// <code>
    /// // 使用 using 确保 Dispose 被调用
    /// using var generator = new DeviceDataGeneratorService(config, logger);
    /// // 方法结束时自动调用 Dispose()
    /// </code>
    /// </summary>
    public void Dispose()
    {
        lock (_stateLock)
        {
            if (_disposed) return;

            _disposed = true;
            // 异步运行方法拥有 CTS；Dispose 仅请求取消，真正释放发生在 StartAsync 的 finally。
            _cts?.Cancel();
        }
    }
}
