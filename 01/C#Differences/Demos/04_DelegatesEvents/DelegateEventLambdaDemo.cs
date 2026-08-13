namespace C_Differences.Demos._04_DelegatesEvents;

/// <summary>
/// 委托、事件、Lambda 演示
/// </summary>
public class DelegateEventLambdaDemo
{
    // 1. 委托定义
    public delegate void LogHandler(string message);
    public delegate int MathOperation(int a, int b);

    // 2. 事件定义
    public event EventHandler<DataEventArgs>? DataReceived;
    public event Action<string>? StatusChanged;

    public void Demo()
    {
        Console.WriteLine("1. 委托基础:");
        DelegateBasics();

        Console.WriteLine("\n2. Lambda 表达式:");
        LambdaBasics();

        Console.WriteLine("\n3. 内置委托类型:");
        BuiltInDelegates();

        Console.WriteLine("\n4. 事件基础:");
        EventBasics();

        Console.WriteLine("\n5. 实际应用 - 简单事件系统:");
        PracticalExample();
    }

    private void DelegateBasics()
    {
        // 委托实例化
        LogHandler logToConsole = Console.WriteLine;
        LogHandler logToUpper = message => Console.WriteLine(message.ToUpper());

        // 使用委托
        logToConsole("   这是控制台日志");
        logToUpper("   这是大写日志");

        // 多播委托
        LogHandler multiLog = logToConsole + logToUpper;
        multiLog("   多播委托消息");

        // 委托作为参数
        int result = Calculate(10, 5, Add);
        Console.WriteLine($"   计算结果 (10 + 5): {result}");

        result = Calculate(10, 5, Multiply);
        Console.WriteLine($"   计算结果 (10 * 5): {result}");
    }

    private void LambdaBasics()
    {
        // Lambda 表达式
        MathOperation subtract = (a, b) => a - b;
        MathOperation divide = (a, b) => a / b;

        Console.WriteLine($"   减法: {subtract(10, 5)}");
        Console.WriteLine($"   除法: {divide(10, 5)}");

        // 多行 Lambda
        MathOperation power = (a, b) =>
        {
            int result = 1;
            for (int i = 0; i < b; i++)
                result *= a;
            return result;
        };
        Console.WriteLine($"   幂运算: {power(2, 3)}");

        // Lambda 捕获变量
        int multiplier = 3;
        Func<int, int> multiply = x => x * multiplier;
        Console.WriteLine($"   乘以 {multiplier}: {multiply(5)}");
        multiplier = 5;
        Console.WriteLine($"   乘以 {multiplier}: {multiply(5)}");
    }

    private void BuiltInDelegates()
    {
        // Action - 无返回值
        Action<string> print = s => Console.WriteLine($"   Action: {s}");
        print("Hello");

        // Func - 有返回值
        Func<int, int, int> add = (a, b) => a + b;
        Console.WriteLine($"   Func: {add(3, 4)}");

        // Predicate - 返回 bool
        Predicate<int> isEven = n => n % 2 == 0;
        Console.WriteLine($"   Predicate (4 是偶数): {isEven(4)}");
        Console.WriteLine($"   Predicate (5 是偶数): {isEven(5)}");

        // 使用内置委托
        var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var evenNumbers = numbers.FindAll(isEven);
        Console.WriteLine($"   偶数: {string.Join(", ", evenNumbers)}");
    }

    private void EventBasics()
    {
        // 订阅事件
        DataReceived += OnDataReceived;
        StatusChanged += OnStatusChanged;

        // 触发事件
        DataReceived?.Invoke(this, new DataEventArgs("传感器数据", 25.5));
        StatusChanged?.Invoke("系统启动");

        // 取消订阅
        DataReceived -= OnDataReceived;
        StatusChanged -= OnStatusChanged;
    }

    private void PracticalExample()
    {
        // 创建温度监控器
        var monitor = new TemperatureMonitor();

        // 订阅事件
        monitor.TemperatureChanged += (sender, temp) =>
        {
            Console.WriteLine($"   温度变化: {temp:F1}°C");
        };

        monitor.HighTemperatureAlert += (sender, temp) =>
        {
            Console.WriteLine($"   ⚠️ 高温警报: {temp:F1}°C");
        };

        // 模拟温度变化
        monitor.SimulateTemperatureChange();
    }

    // 辅助方法
    private int Calculate(int a, int b, MathOperation operation)
    {
        return operation(a, b);
    }

    private int Add(int a, int b) => a + b;
    private int Multiply(int a, int b) => a * b;

    // 事件处理方法
    private void OnDataReceived(object? sender, DataEventArgs e)
    {
        Console.WriteLine($"   收到数据: {e.DataType} = {e.Value}");
    }

    private void OnStatusChanged(string status)
    {
        Console.WriteLine($"   状态变化: {status}");
    }
}

/// <summary>
/// 事件参数类
/// </summary>
public class DataEventArgs : EventArgs
{
    public string DataType { get; }
    public double Value { get; }

    public DataEventArgs(string dataType, double value)
    {
        DataType = dataType;
        Value = value;
    }
}

/// <summary>
/// 温度监控器 - 实际应用示例
/// </summary>
public class TemperatureMonitor
{
    private double _currentTemperature;
    private readonly Random _random = new();

    public event EventHandler<double>? TemperatureChanged;
    public event EventHandler<double>? HighTemperatureAlert;

    public double CurrentTemperature
    {
        get => _currentTemperature;
        private set
        {
            if (_currentTemperature != value)
            {
                _currentTemperature = value;
                TemperatureChanged?.Invoke(this, value);

                if (value > 30)
                {
                    HighTemperatureAlert?.Invoke(this, value);
                }
            }
        }
    }

    public void SimulateTemperatureChange()
    {
        Console.WriteLine("   模拟温度变化:");
        for (int i = 0; i < 5; i++)
        {
            CurrentTemperature = 20 + _random.NextDouble() * 15; // 20-35°C
            Thread.Sleep(100);
        }
    }
}
