namespace C_Differences.Demos._06_AsyncAwait;

/// <summary>
/// Task 和 async/await 演示
///
/// 【Java 对比】
/// - Java: CompletableFuture&lt;T&gt; + .thenApply() / .thenCompose() 链式调用
/// - C#:   Task&lt;T&gt; + async/await（语法糖，编译器自动生成状态机）
///
/// 【关键区别 - 必须理解】
/// 1. Task ≠ 线程！
///    - Task 是一个"承诺"（Promise），表示异步操作的最终结果
///    - Task 可能在线程池线程上执行，也可能不使用新线程
///    - 类比：Task 像快递单，线程像快递员
///
/// 2. async/await 让异步代码看起来像同步代码
///    - Java 的 CompletableFuture 需要链式调用，代码难以阅读
///    - C# 的 await 会"暂停"方法，但不阻塞线程
///
/// 3. await 不会阻塞线程
///    - await 会将后续代码注册为回调，然后释放当前线程
///    - 当异步操作完成时，回调会在合适的线程上执行
/// </summary>
public class TaskAsyncDemo
{
    /// <summary>
    /// 演示入口 - 按顺序展示各种异步编程模式
    /// </summary>
    public async Task DemoAsync()
    {
        Console.WriteLine("1. 基础 Task 创建与 await:");
        await BasicTaskAsync();

        Console.WriteLine("\n2. Task 返回值:");
        await TaskWithResultAsync();

        Console.WriteLine("\n3. 并行执行多个 Task:");
        await ParallelTasksAsync();

        Console.WriteLine("\n4. Task 组合（类似 Java thenCompose）:");
        await TaskCompositionAsync();

        Console.WriteLine("\n5. 错误处理:");
        await ErrorHandlingAsync();

        Console.WriteLine("\n6. 实际应用 - 模拟设备数据采集:");
        await PracticalExampleAsync();
    }

    /// <summary>
    /// 基础 Task：异步但不返回值
    ///
    /// 【知识点】
    /// - Task.Delay(ms): 异步版本的 Thread.Sleep，不阻塞线程
    /// - await: 暂停方法执行，等待 Task 完成后继续
    /// - async 方法返回 Task: 表示这是一个异步方法
    /// </summary>
    private async Task BasicTaskAsync()
    {
        Console.WriteLine("   开始执行...");
        Console.WriteLine($"   线程 ID: {Thread.CurrentThread.ManagedThreadId}");

        // await: 释放当前线程，等待操作完成后继续
        // 注意：await 前后可能在不同线程上执行！
        await Task.Delay(100); // 类似 Thread.sleep，但不阻塞线程

        Console.WriteLine($"   100ms 后继续，线程 ID: {Thread.CurrentThread.ManagedThreadId}");

        // 模拟耗时操作
        await SimulateWorkAsync("数据库查询", 200);
        Console.WriteLine("   数据库查询完成");
    }

    /// <summary>
    /// Task&lt;T&gt;：异步且返回值
    ///
    /// 【知识点】
    /// - Task&lt;T&gt;: 表示异步操作的结果，类似 Java 的 CompletableFuture&lt;T&gt;
    /// - await Task&lt;T&gt;: 会返回 T 类型的值
    /// </summary>
    private async Task TaskWithResultAsync()
    {
        // 获取异步结果 - await 会"拆包" Task&lt;int&gt; 得到 int
        int result = await CalculateAsync(10, 20);
        Console.WriteLine($"   计算结果: {result}");

        // 获取传感器数据
        double temperature = await ReadSensorAsync("温度传感器");
        Console.WriteLine($"   温度读数: {temperature:F1}°C");
    }

    /// <summary>
    /// 并行执行：同时启动多个 Task
    ///
    /// 【重要模式 - 并行 vs 串行】
    /// 串行：await A; await B; await C;  // 总时间 = A + B + C
    /// 并行：var a = A; var b = B; var c = C; await WhenAll(a, b, c);  // 总时间 = max(A, B, C)
    /// </summary>
    private async Task ParallelTasksAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // ❌ 错误示范（串行执行）：
        // var temp = await ReadSensorAsync("温度");      // 等待 300ms
        // var pressure = await ReadSensorAsync("压力");   // 再等待 300ms
        // var speed = await ReadSensorAsync("转速");      // 再等待 300ms
        // 总时间 = 300 + 300 + 300 = 900ms

        // ✅ 正确做法：先启动所有 Task（不 await），再统一 await
        Task<double> tempTask = ReadSensorAsync("温度传感器");      // 立即返回 Task
        Task<double> pressureTask = ReadSensorAsync("压力传感器");  // 立即返回 Task
        Task<double> speedTask = ReadSensorAsync("转速传感器");     // 立即返回 Task

        // Task.WhenAll: 等待所有 Task 完成
        double[] results = await Task.WhenAll(tempTask, pressureTask, speedTask);

        stopwatch.Stop();
        Console.WriteLine($"   温度: {results[0]:F1}°C, 压力: {results[1]:F1}MPa, 转速: {results[2]:F1}rpm");
        Console.WriteLine($"   并行执行耗时: {stopwatch.ElapsedMilliseconds}ms (约300ms，而非900ms)");
    }

    /// <summary>
    /// Task 组合：一个 Task 的结果作为另一个的输入
    ///
    /// 【Java 对比】
    /// Java CompletableFuture 链式调用：
    /// <code>
    /// readSensor("温度传感器")
    ///     .thenCompose(raw -> calibrateSensor(raw))
    ///     .thenAccept(calibrated -> System.out.println(calibrated));
    /// </code>
    ///
    /// C# 的写法像同步代码，更易读：
    /// <code>
    /// var raw = await ReadSensorAsync("温度传感器");
    /// var calibrated = await CalibrateSensorAsync(raw);
    /// Console.WriteLine(calibrated);
    /// </code>
    /// </summary>
    private async Task TaskCompositionAsync()
    {
        // C# 的写法：像同步代码一样自然
        double rawValue = await ReadSensorAsync("温度传感器");
        double calibrated = await CalibrateSensorAsync(rawValue);
        string formatted = FormatReading(calibrated, "°C");

        Console.WriteLine($"   原始值: {rawValue:F1} → 校准值: {calibrated:F1} → 格式化: {formatted}");
    }

    /// <summary>
    /// 错误处理：Task 中的异常
    ///
    /// 【知识点】
    /// - Task 中的异常会在 await 时重新抛出
    /// - 可以用 try-catch 捕获（最常用）
    /// - Task.WhenAll 只抛出第一个异常，需要检查各个 Task
    /// </summary>
    private async Task ErrorHandlingAsync()
    {
        // 方式 1：try-catch（最常用）
        try
        {
            await SimulateFailingOperationAsync();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   捕获异常: {ex.Message}");
        }

        // 方式 2：Task.WhenAll 的异常处理
        Task<int> task1 = SafeCalculateAsync(10, 0);
        Task<int> task2 = SafeCalculateAsync(20, 0);

        Task[] tasks = { task1, task2 };
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
            // WhenAll 只抛出第一个异常，需要检查各个 Task
            foreach (var task in tasks)
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine($"   任务失败: {task.Exception?.InnerException?.Message}");
                }
            }
        }

        // 方式 3：检查 Task 状态
        var resultTask = SafeCalculateAsync(10, 2);
        await resultTask;

        if (resultTask.Status == TaskStatus.RanToCompletion)
        {
            Console.WriteLine($"   计算成功: {resultTask.Result}");
        }
    }

    /// <summary>
    /// 实际应用：模拟设备数据采集
    /// </summary>
    private async Task PracticalExampleAsync()
    {
        Console.WriteLine("   启动设备数据采集...");

        // 模拟初始化
        await InitializeDeviceAsync();

        // 连续采集 5 次
        for (int i = 0; i < 5; i++)
        {
            var reading = await ReadDeviceDataAsync();
            Console.WriteLine($"   [{reading.Timestamp:HH:mm:ss}] 温度={reading.Temperature:F1}°C " +
                            $"压力={reading.Pressure:F1}MPa 转速={reading.Speed:F0}rpm");
        }

        Console.WriteLine("   数据采集完成");
    }

    // ========== 辅助方法 ==========

    /// <summary>
    /// 模拟耗时操作
    /// </summary>
    /// <param name="name">操作名称</param>
    /// <param name="delayMs">延迟毫秒数</param>
    private async Task SimulateWorkAsync(string name, int delayMs)
    {
        Console.WriteLine($"   [{name}] 开始...");
        await Task.Delay(delayMs);
        Console.WriteLine($"   [{name}] 完成");
    }

    /// <summary>
    /// 异步计算 - 返回 Task&lt;int&gt;
    /// </summary>
    private async Task<int> CalculateAsync(int a, int b)
    {
        await Task.Delay(50); // 模拟异步计算
        return a + b;
    }

    /// <summary>
    /// 读取传感器数据 - 返回 Task&lt;double&gt;
    /// </summary>
    /// <param name="sensorName">传感器名称</param>
    /// <returns>传感器读数</returns>
    private async Task<double> ReadSensorAsync(string sensorName)
    {
        await Task.Delay(300); // 模拟传感器读取延迟
        return Random.Shared.NextDouble() * 100;
    }

    /// <summary>
    /// 校准传感器数据
    /// </summary>
    /// <param name="rawValue">原始值</param>
    /// <returns>校准后的值</returns>
    private async Task<double> CalibrateSensorAsync(double rawValue)
    {
        await Task.Delay(50); // 模拟校准计算
        return rawValue * 0.95 + 2.5; // 简单校准公式
    }

    /// <summary>
    /// 格式化读数显示
    /// </summary>
    private string FormatReading(double value, string unit)
    {
        return $"{value:F2}{unit}";
    }

    /// <summary>
    /// 模拟失败的操作 - 用于演示异常处理
    /// </summary>
    private async Task SimulateFailingOperationAsync()
    {
        await Task.Delay(50);
        throw new InvalidOperationException("设备连接超时");
    }

    /// <summary>
    /// 安全的计算方法 - 可能抛出异常
    /// </summary>
    private async Task<int> SafeCalculateAsync(int a, int b)
    {
        await Task.Delay(50);
        if (b == 0)
            throw new DivideByZeroException("除数不能为零");
        return a / b;
    }

    /// <summary>
    /// 初始化设备
    /// </summary>
    private async Task InitializeDeviceAsync()
    {
        Console.WriteLine("   初始化设备...");
        await Task.Delay(200);
        Console.WriteLine("   设备就绪");
    }

    /// <summary>
    /// 读取设备数据
    /// </summary>
    private async Task<DeviceReading> ReadDeviceDataAsync()
    {
        await Task.Delay(200); // 模拟读取延迟
        return new DeviceReading
        {
            Timestamp = DateTimeOffset.UtcNow,
            Temperature = 20 + Random.Shared.NextDouble() * 15,
            Pressure = 0.1 + Random.Shared.NextDouble() * 0.5,
            Speed = 1000 + Random.Shared.NextDouble() * 2000
        };
    }
}

/// <summary>
/// 设备读数记录 - 使用 record 实现不可变数据
///
/// 【知识点】
/// - record: C# 9.0 引入的引用类型，适合存储数据
/// - init: 只在初始化时可赋值，之后不可修改
/// </summary>
public record DeviceReading
{
    /// <summary>时间戳（UTC时间，避免时区歧义）</summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>温度 (°C)</summary>
    public double Temperature { get; init; }

    /// <summary>压力 (MPa)</summary>
    public double Pressure { get; init; }

    /// <summary>转速 (rpm)</summary>
    public double Speed { get; init; }
}
