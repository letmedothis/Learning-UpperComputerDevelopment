namespace C_Differences.Demos._07_Disposable;

/// <summary>
/// IDisposable 演示
///
/// Java 对比：
/// - Java: AutoCloseable + try-with-resources
/// - C#:   IDisposable + using 语句/声明
///
/// 核心概念：
/// - IDisposable: 接口，定义 Dispose() 方法用于释放非托管资源
/// - using 语句: 确保 Dispose() 在作用域结束时被调用（类似 Java try-with-resources）
/// - 非托管资源: 文件句柄、数据库连接、网络连接、Socket 等
///
/// 什么时候需要实现 IDisposable：
/// 1. 类直接持有非托管资源
/// 2. 类的字段中有其他 IDisposable 对象
/// </summary>
public class DisposableDemo
{
    public void Demo()
    {
        Console.WriteLine("1. 基础 using 语句:");
        BasicUsing();

        Console.WriteLine("\n2. using 声明 (C# 8.0+):");
        UsingDeclaration();

        Console.WriteLine("\n3. 自定义 IDisposable:");
        CustomDisposable();

        Console.WriteLine("\n4. 实际应用 - 文件操作:");
        FileOperationExample();

        Console.WriteLine("\n5. 实际应用 - 模拟数据库连接:");
        DatabaseExample();
    }

    /// <summary>
    /// 基础 using 语句
    /// </summary>
    private void BasicUsing()
    {
        // using 语句确保在块结束时调用 Dispose()
        // 等价于 Java 的 try-with-resources
        using (var resource = new SimpleResource("基础资源"))
        {
            resource.DoWork();
        } // 这里自动调用 Dispose()

        Console.WriteLine("   using 块结束，资源已释放");
    }

    /// <summary>
    /// using 声明 (C# 8.0+) - 更简洁的语法
    /// </summary>
    private void UsingDeclaration()
    {
        // using 声明：变量在作用域结束时自动释放
        using var resource = new SimpleResource("声明式资源");
        resource.DoWork();

        // 不需要显式的 using 块，方法结束时自动释放
        Console.WriteLine("   方法结束时自动释放");
    }

    /// <summary>
    /// 自定义 IDisposable 实现
    /// </summary>
    private void CustomDisposable()
    {
        using var connection = new FakeDatabaseConnection("Server=localhost;Database=Test");
        connection.Open();
        connection.ExecuteQuery("SELECT * FROM Sensors");
        connection.ExecuteQuery("INSERT INTO Logs VALUES ('test')");
        // 连接在方法结束时自动关闭
    }

    /// <summary>
    /// 文件操作示例
    /// </summary>
    private void FileOperationExample()
    {
        string tempFile = Path.GetTempFileName();

        try
        {
            // StreamWriter 实现了 IDisposable
            using (var writer = new StreamWriter(tempFile))
            {
                writer.WriteLine("温度,压力,转速");
                writer.WriteLine("25.5,0.3,1500");
                writer.WriteLine("26.1,0.4,1600");
            }

            // StreamReader 也实现了 IDisposable
            using (var reader = new StreamReader(tempFile))
            {
                Console.WriteLine("   文件内容:");
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine($"     {line}");
                }
            }
        }
        finally
        {
            // 清理临时文件
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    /// <summary>
    /// 模拟数据库连接
    /// </summary>
    private void DatabaseExample()
    {
        using var db = new FakeDatabaseConnection("Server=localhost;Database=Sensors");
        db.Open();

        // 模拟批量操作
        for (int i = 0; i < 3; i++)
        {
            var sensorData = $"传感器{i + 1}: 温度={20 + i}°C";
            db.ExecuteQuery($"INSERT INTO Readings VALUES ('{sensorData}')");
        }

        Console.WriteLine("   批量操作完成，连接将自动关闭");
    }
}

/// <summary>
/// 简单的资源类 - 演示 IDisposable 基础
/// </summary>
public class SimpleResource : IDisposable
{
    private readonly string _name;
    private bool _disposed = false;

    public SimpleResource(string name)
    {
        _name = name;
        Console.WriteLine($"   [{_name}] 资源创建");
    }

    public void DoWork()
    {
        if (_disposed)
            throw new ObjectDisposedException(_name);

        Console.WriteLine($"   [{_name}] 执行工作...");
    }

    /// <summary>
    /// 释放资源 - IDisposable 接口方法
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine($"   [{_name}] 资源释放");
            _disposed = true;
        }
    }
}

/// <summary>
/// 模拟数据库连接 - 更实际的示例
/// </summary>
public class FakeDatabaseConnection : IDisposable
{
    private readonly string _connectionString;
    private bool _isOpen = false;
    private bool _disposed = false;

    public FakeDatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
        Console.WriteLine($"   [DB] 连接对象创建: {(connectionString.Length > 20 ? connectionString[..20] : connectionString)}...");
    }

    public void Open()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FakeDatabaseConnection));

        _isOpen = true;
        Console.WriteLine("   [DB] 连接已打开");
    }

    public void ExecuteQuery(string query)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FakeDatabaseConnection));

        if (!_isOpen)
            throw new InvalidOperationException("连接未打开");

        Console.WriteLine($"   [DB] 执行: {query}");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_isOpen)
            {
                _isOpen = false;
                Console.WriteLine("   [DB] 连接已关闭");
            }
            _disposed = true;
            Console.WriteLine("   [DB] 连接对象已释放");
        }
    }
}
