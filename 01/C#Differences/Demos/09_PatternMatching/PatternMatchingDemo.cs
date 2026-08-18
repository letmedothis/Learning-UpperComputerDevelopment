namespace C_Differences.Demos._09_PatternMatching;

/// <summary>
/// 模式匹配演示 - C# 最强大的特性之一
///
/// 【为什么模式匹配很重要】
/// 模式匹配是 C# 7.0+ 引入的核心特性，它让代码更简洁、更安全、更具表达力。
/// 对于 Java 开发者来说，这是 C# 最值得学习的特性之一。
///
/// 【Java 对比总览】
/// ┌─────────────────┬──────────────────────────────────────────────────────────────────┐
/// │ 模式类型         │ Java 等价物                                                      │
/// ├─────────────────┼──────────────────────────────────────────────────────────────────┤
/// │ 类型模式         │ Java 16+: if (obj instanceof String s)                           │
/// │ 常量模式         │ Java: if (status == null) 或 if (status.equals("running"))        │
/// │ 关系模式         │ Java: 无直接等价物，需要 if-else 链                               │
/// │ 逻辑模式         │ Java: if (x > 0 && x < 100)                                      │
/// │ 属性模式         │ Java: 无直接等价物，需要手动检查每个属性                           │
/// │ 位置模式         │ Java: 无直接等价物                                                │
/// │ switch 表达式   │ Java 14+: switch 表达式                                          │
/// └─────────────────┴──────────────────────────────────────────────────────────────────┘
///
/// 【C# 模式匹配的完整语法】
/// <code>
/// // 1. is 模式
/// if (obj is string s) { ... }
/// if (obj is null) { ... }
/// if (obj is > 0 and < 100) { ... }
///
/// // 2. switch 表达式
/// string result = value switch
/// {
///     int i when i > 0 => "正整数",
///     string s => "字符串",
///     null => "null",
///     _ => "其他"
/// };
///
/// // 3. 属性模式
/// if (reading is { Temperature: > 40, Pressure: > 0.6 }) { ... }
///
/// // 4. 位置模式
/// if (point is (0, 0)) { ... }
/// </code>
///
/// 【学习建议】
/// 1. 先掌握类型模式和常量模式（最常用）
/// 2. 再学习关系模式和逻辑模式（简化条件判断）
/// 3. 最后掌握属性模式和位置模式（高级用法）
/// 4. 在实际项目中多使用 switch 表达式替代 if-else 链
/// </summary>
public class PatternMatchingDemo
{
    public void Demo()
    {
        Console.WriteLine("=== C# 模式匹配学习 ===\n");

        Console.WriteLine("1. 类型模式 (Type Pattern):");
        TypePatternDemo();

        Console.WriteLine("\n2. 常量模式 (Constant Pattern):");
        ConstantPatternDemo();

        Console.WriteLine("\n3. 关系模式 (Relational Pattern):");
        RelationalPatternDemo();

        Console.WriteLine("\n4. 逻辑模式 (Logical Pattern):");
        LogicalPatternDemo();

        Console.WriteLine("\n5. 属性模式 (Property Pattern):");
        PropertyPatternDemo();

        Console.WriteLine("\n6. switch 表达式:");
        SwitchExpressionDemo();

        Console.WriteLine("\n7. 位置模式 (Positional Pattern):");
        PositionalPatternDemo();

        Console.WriteLine("\n8. 实际应用 - 设备状态判断:");
        PracticalExample();
    }

    /// <summary>
    /// 类型模式 - 检查类型并提取值
    ///
    /// 【Java 对比】
    /// Java: if (obj instanceof String s) { ... }
    /// C#:   if (obj is string s) { ... }
    /// </summary>
    private void TypePatternDemo()
    {
        object[] values = { 42, "Hello", 3.14, true, new DeviceReading() };

        foreach (var obj in values)
        {
            // is 模式：检查类型并绑定变量
            if (obj is int number)
            {
                Console.WriteLine($"   整数: {number}");
            }
            else if (obj is string text)
            {
                Console.WriteLine($"   字符串: {text} (长度: {text.Length})");
            }
            else if (obj is double real)
            {
                Console.WriteLine($"   浮点数: {real:F2}");
            }
            else if (obj is bool flag)
            {
                Console.WriteLine($"   布尔值: {flag}");
            }
            else
            {
                Console.WriteLine($"   其他类型: {obj.GetType().Name}");
            }
        }
    }

    /// <summary>
    /// 常量模式 - 与常量值比较
    ///
    /// 【Java 对比】
    /// Java: if (status == null) { ... }
    /// C#:   if (status is null) { ... }  // 推荐，避免 == 运算符重载问题
    /// </summary>
    private void ConstantPatternDemo()
    {
        string? status = "running";

        // null 检查
        if (status is null)
        {
            Console.WriteLine("   状态为 null");
        }
        else if (status is "")
        {
            Console.WriteLine("   状态为空字符串");
        }
        else if (status is "running")
        {
            Console.WriteLine("   状态为运行中");
        }
        else if (status is "stopped")
        {
            Console.WriteLine("   状态为已停止");
        }

        // 数字常量
        int code = 404;
        if (code is 200)
            Console.WriteLine("   成功");
        else if (code is 404)
            Console.WriteLine("   未找到");
        else if (code is 500)
            Console.WriteLine("   服务器错误");
    }

    /// <summary>
    /// 关系模式 - 使用比较运算符
    ///
    /// 【Java 对比】
    /// Java 没有直接等价物，需要使用 if-else 链
    /// C# 可以在 switch 表达式中使用关系运算符
    /// </summary>
    private void RelationalPatternDemo()
    {
        int temperature = 35;

        // 关系模式：<, >, <=, >=
        string status = temperature switch
        {
            < 0 => "极寒",
            >= 0 and < 10 => "寒冷",
            >= 10 and < 20 => "凉爽",
            >= 20 and < 30 => "舒适",
            >= 30 and < 40 => "炎热",
            >= 40 => "酷热"
        };
        Console.WriteLine($"   温度 {temperature}°C: {status}");

        // 浮点数关系模式
        double pressure = 0.45;
        string pressureStatus = pressure switch
        {
            < 0.1 => "过低",
            >= 0.1 and <= 0.5 => "正常",
            > 0.5 and <= 0.6 => "偏高",
            > 0.6 => "过高"
        };
        Console.WriteLine($"   压力 {pressure} MPa: {pressureStatus}");
    }

    /// <summary>
    /// 逻辑模式 - 组合多个条件
    ///
    /// 【Java 对比】
    /// Java: if (x > 0 && x < 100) { ... }
    /// C#:   if (x is > 0 and < 100) { ... }
    /// </summary>
    private void LogicalPatternDemo()
    {
        // and 模式：同时满足
        int age = 25;
        if (age is >= 18 and <= 65)
        {
            Console.WriteLine($"   年龄 {age}: 工作年龄");
        }

        // or 模式：满足其一
        string day = "周六";
        if (day is "周六" or "周日")
        {
            Console.WriteLine($"   {day}: 周末");
        }

        // not 模式：取反
        string? name = "张三";
        if (name is not null)
        {
            Console.WriteLine($"   姓名: {name}");
        }

        // 组合模式
        int score = 85;
        string grade = score switch
        {
            >= 90 => "A",
            >= 80 and < 90 => "B",
            >= 70 and < 80 => "C",
            >= 60 and < 70 => "D",
            < 60 => "F"
        };
        Console.WriteLine($"   分数 {score}: 等级 {grade}");
    }

    /// <summary>
    /// 属性模式 - 检查对象的属性值
    ///
    /// 【Java 对比】
    /// Java 没有直接等价物，需要手动检查每个属性
    /// C# 可以在模式中直接检查属性
    /// </summary>
    private void PropertyPatternDemo()
    {
        var reading = new DeviceReading
        {
            Temperature = 38.5,
            Pressure = 0.55,
            Speed = 1800
        };

        // 属性模式：检查单个属性
        if (reading is { Temperature: > 35 })
        {
            Console.WriteLine($"   温度偏高: {reading.Temperature}°C");
        }

        // 嵌套属性模式：检查多个属性
        string status = reading switch
        {
            { Temperature: > 40, Pressure: > 0.6 } => "危险状态！",
            { Temperature: > 35 } => "温度偏高",
            { Pressure: > 0.5 } => "压力偏高",
            { Speed: > 2000 } => "转速偏高",
            _ => "正常"
        };
        Console.WriteLine($"   设备状态: {status}");

        // 带类型检查的属性模式
        object obj = new { Name = "传感器1", Value = 25.5 };
        if (obj is { } anonymous)
        {
            Console.WriteLine($"   匿名类型: {anonymous}");
        }
    }

    /// <summary>
    /// switch 表达式 - 更简洁的 switch 语法
    ///
    /// 【Java 对比】
    /// Java 14+ switch 表达式：
    /// <code>
    /// String result = switch (day) {
    ///     case "MONDAY" -> "周一";
    ///     case "TUESDAY" -> "周二";
    ///     default -> "其他";
    /// };
    /// </code>
    ///
    /// C# 的 switch 表达式更强大，支持模式匹配
    /// </summary>
    private void SwitchExpressionDemo()
    {
        // 基本 switch 表达式
        string day = "周三";
        string dayType = day switch
        {
            "周一" or "周二" or "周三" or "周四" or "周五" => "工作日",
            "周六" or "周日" => "周末",
            _ => "未知"
        };
        Console.WriteLine($"   {day}: {dayType}");

        // 带模式匹配的 switch 表达式
        object value = 42;
        string description = value switch
        {
            int i when i > 0 => $"正整数: {i}",
            int i when i < 0 => $"负整数: {i}",
            int => "零",
            string s => $"字符串: {s}",
            null => "null",
            _ => $"其他: {value.GetType().Name}"
        };
        Console.WriteLine($"   值描述: {description}");

        // 元组模式
        int x = 5, y = 10;
        string quadrant = (x, y) switch
        {
            (> 0, > 0) => "第一象限",
            (< 0, > 0) => "第二象限",
            (< 0, < 0) => "第三象限",
            (> 0, < 0) => "第四象限",
            (0, 0) => "原点",
            _ => "坐标轴上"
        };
        Console.WriteLine($"   ({x}, {y}): {quadrant}");
    }

    /// <summary>
    /// 位置模式 - 使用解构函数
    ///
    /// 【Java 对比】
    /// Java 没有直接等价物
    /// C# 可以对实现 Deconstruct 方法的类型使用位置模式
    /// </summary>
    private void PositionalPatternDemo()
    {
        var point = new Point(3, 4);

        // 位置模式：使用解构函数
        string location = point switch
        {
            (0, 0) => "原点",
            (_, 0) => "X轴上",
            (0, _) => "Y轴上",
            (> 0, > 0) => "第一象限",
            (< 0, > 0) => "第二象限",
            (< 0, < 0) => "第三象限",
            (> 0, < 0) => "第四象限"
        };
        Console.WriteLine($"   点 ({point.X}, {point.Y}): {location}");

        // 带变量绑定的位置模式
        if (point is (var px, var py) and not (0, 0))
        {
            double distance = Math.Sqrt(px * px + py * py);
            Console.WriteLine($"   到原点距离: {distance:F2}");
        }
    }

    /// <summary>
    /// 实际应用 - 设备状态判断
    /// </summary>
    private void PracticalExample()
    {
        var readings = new[]
        {
            new DeviceReading { Temperature = 25.0, Pressure = 0.3, Speed = 1500 },
            new DeviceReading { Temperature = 38.5, Pressure = 0.55, Speed = 2200 },
            new DeviceReading { Temperature = 42.0, Pressure = 0.65, Speed = 2600 },
            new DeviceReading { Temperature = 10.0, Pressure = 0.08, Speed = 800 }
        };

        foreach (var reading in readings)
        {
            // 使用属性模式和逻辑模式进行综合判断
            string diagnosis = reading switch
            {
                { Temperature: > 40, Pressure: > 0.6 } =>
                    "🔴 严重警告：温度和压力同时超限！立即停机！",
                { Temperature: > 40 } =>
                    "🟠 温度报警：温度过高，需要检查冷却系统",
                { Pressure: > 0.6 } =>
                    "🟠 压力报警：压力过高，需要检查泄压阀",
                { Speed: > 2500 } =>
                    "🟠 转速报警：转速过高，需要检查传动系统",
                { Temperature: < 15 } =>
                    "🟡 温度偏低：可能需要预热",
                { Pressure: < 0.1 } =>
                    "🟡 压力偏低：可能有泄漏",
                { Speed: < 1000 } =>
                    "🟡 转速偏低：可能需要维护",
                _ => "🟢 正常"
            };

            Console.WriteLine($"   温度={reading.Temperature:F1}°C, 压力={reading.Pressure:F3}MPa, 转速={reading.Speed:F0}rpm");
            Console.WriteLine($"   诊断: {diagnosis}\n");
        }
    }
}

/// <summary>
/// 设备读数类 - 用于演示
/// </summary>
public class DeviceReading
{
    public double Temperature { get; init; }
    public double Pressure { get; init; }
    public double Speed { get; init; }
}

/// <summary>
/// 点类型 - 演示 Deconstruct 方法
/// </summary>
public class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// 解构函数 - 允许使用位置模式
    /// </summary>
    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }
}
