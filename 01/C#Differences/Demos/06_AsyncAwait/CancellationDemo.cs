namespace C_Differences.Demos._06_AsyncAwait;

/// <summary>
/// CancellationToken 演示
///
/// Java 对比：
/// - Java 没有内置的取消机制，通常用 volatile boolean flag 或 Thread.interrupt()
/// - C# 通过 CancellationTokenSource + CancellationToken 提供统一的协作式取消
///
/// 核心概念：
/// - CancellationTokenSource: 取消的"控制者"，调用 Cancel() 发出取消信号
/// - CancellationToken: 取消的"令牌"，传递给异步方法，方法内部检查是否被取消
/// - 协作式取消: 异步方法主动检查令牌，不是强制终止线程
/// </summary>
public class CancellationDemo
{
    public async Task DemoAsync()
    {
        Console.WriteLine("1. 基础取消:");
        await BasicCancellationAsync();

        Console.WriteLine("\n2. 超时取消:");
        await TimeoutCancellationAsync();

        Console.WriteLine("\n3. 链式取消令牌:");
        await LinkedCancellationAsync();

        Console.WriteLine("\n4. 实际应用 - 设备数据采集（可取消）:");
        await PracticalExampleAsync();
    }

    /// <summary>
    /// 基础取消：手动触发取消
    /// </summary>
    private async Task BasicCancellationAsync()
    {
        // 创建 CancellationTokenSource
        using var cts = new CancellationTokenSource();

        // 启动长时间运行的任务
        var task = LongRunningOperationAsync(cts.Token);

        // 等待 500ms 后取消
        await Task.Delay(500);
        Console.WriteLine("   发送取消信号...");
        cts.Cancel();

        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("   任务已被取消 (OperationCanceledException)");
        }
    }

    /// <summary>
    /// 超时取消：自动在指定时间后取消
    /// </summary>
    private async Task TimeoutCancellationAsync()
    {
        // 方式 1：使用 CancelAfter
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(300)); // 300ms 后自动取消

        try
        {
            await LongRunningOperationAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("   任务超时被取消");
        }

        // 方式 2：使用 CancellationTokenSource 的构造函数
        using var cts2 = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
        try
        {
            await LongRunningOperationAsync(cts2.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("   任务超时被取消 (构造函数方式)");
        }
    }

    /// <summary>
    /// 链式取消：多个取消条件组合
    /// </summary>
    private async Task LinkedCancellationAsync()
    {
        using var userCts = new CancellationTokenSource();      // 用户取消
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(800)); // 超时

        // 链接：任一取消都会触发
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            userCts.Token,
            timeoutCts.Token
        );

        // 模拟用户在 400ms 后手动取消
        _ = Task.Delay(400).ContinueWith(_ =>
        {
            Console.WriteLine("   用户请求取消...");
            userCts.Cancel();
        });

        try
        {
            await LongRunningOperationAsync(linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
            // 检查取消原因
            if (userCts.IsCancellationRequested)
                Console.WriteLine("   任务因用户取消而终止");
            else if (timeoutCts.IsCancellationRequested)
                Console.WriteLine("   任务因超时而终止");
        }
    }

    /// <summary>
    /// 实际应用：可取消的设备数据采集
    /// </summary>
    private async Task PracticalExampleAsync()
    {
        using var cts = new CancellationTokenSource();

        // 启动数据采集
        var collectionTask = CollectDeviceDataAsync(cts.Token);

        // 采集 1 秒后停止
        await Task.Delay(1000);
        Console.WriteLine("\n   停止数据采集...");
        cts.Cancel();

        try
        {
            await collectionTask;
            Console.WriteLine("   数据采集正常结束");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("   数据采集已取消，资源已清理");
        }
    }

    // ========== 辅助方法 ==========

    /// <summary>
    /// 模拟长时间运行的操作
    /// </summary>
    private async Task LongRunningOperationAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("   长时间任务开始...");

        for (int i = 0; i < 10; i++)
        {
            // 关键：检查取消令牌
            // ThrowIfCancellationRequested() 在取消时抛出 OperationCanceledException
            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine($"   执行步骤 {i + 1}/10");
            await Task.Delay(200, cancellationToken); // Delay 也接受取消令牌
        }

        Console.WriteLine("   长时间任务完成");
    }

    /// <summary>
    /// 模拟设备数据采集（带取消支持）
    /// </summary>
    private async Task CollectDeviceDataAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("   开始设备数据采集 (按任意键停止)...");

        int count = 0;
        while (!cancellationToken.IsCancellationRequested) // 方式 1：检查属性
        {
            count++;
            var reading = new DeviceReading
            {
                Timestamp = DateTime.Now,
                Temperature = 20 + Random.Shared.NextDouble() * 15,
                Pressure = 0.1 + Random.Shared.NextDouble() * 0.5,
                Speed = 1000 + Random.Shared.NextDouble() * 2000
            };

            Console.WriteLine($"   [#{count}] {reading.Timestamp:HH:mm:ss.fff} " +
                            $"温度={reading.Temperature:F1}°C " +
                            $"压力={reading.Pressure:F2}MPa " +
                            $"转速={reading.Speed:F0}rpm");

            try
            {
                await Task.Delay(200, cancellationToken); // 方式 2：传递给 Delay
            }
            catch (OperationCanceledException)
            {
                // Delay 被取消时会抛出异常
                Console.WriteLine("   采集循环被取消");
                throw; // 重新抛出，让调用者知道任务被取消了
            }
        }

        Console.WriteLine($"   采集结束，共采集 {count} 条数据");
    }
}
