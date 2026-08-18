namespace C_Differences.Demos._10_CommonTraps;

/// <summary>
/// Java 开发者常见陷阱和迁移警告
///
/// 【为什么需要这个模块】
/// 从 Java 转到 C# 时，有些概念看起来相似但行为不同，
/// 这些差异很容易导致 bug。本模块帮助你提前了解这些陷阱。
///
/// 【陷阱总览】
/// ┌────┬─────────────────────┬─────────────────────────────────────────────────────────┐
/// │ #  │ 陷阱名称            │ 核心差异                                                │
/// ├────┼─────────────────────┼─────────────────────────────────────────────────────────┤
/// │ 1  │ == vs .Equals()     │ C# string 的 == 比较值，Java 的 == 比较引用             │
/// │ 2  │ string 行为         │ C# string 重载了 ==，行为更直观                         │
/// │ 3  │ null 检查           │ C# 推荐使用 is null，避免运算符重载影响                  │
/// │ 4  │ 值类型 vs 引用类型  │ C# struct 是值类型，赋值时会复制整个值                   │
/// │ 5  │ 异步同步上下文      │ C# await 会捕获同步上下文，可能导致死锁                  │
/// │ 6  │ LINQ 延迟执行       │ C# IEnumerable 每次枚举都会重新计算                     │
/// │ 7  │ 闭包变量捕获        │ C# Lambda 捕获变量本身，不是值                          │
/// │ 8  │ 类型转换            │ C# 有多种转换方式，行为不同                              │
/// └────┴─────────────────────┴─────────────────────────────────────────────────────────┘
///
/// 【学习建议】
/// 1. 重点理解陷阱 1-3（最常遇到）
/// 2. 理解陷阱 4-5（影响程序正确性）
/// 3. 了解陷阱 6-8（避免性能问题和意外行为）
/// 4. 在实际项目中遇到问题时，回来查阅本模块
/// </summary>
public class CommonTrapsDemo
{
    public void Demo()
    {
        Console.WriteLine("=== Java 开发者常见陷阱 ===\n");

        Console.WriteLine("1. == vs .Equals() 陷阱:");
        EqualityTrap();

        Console.WriteLine("\n2. string 是引用类型但表现像值类型:");
        StringTrap();

        Console.WriteLine("\n3. null 检查的差异:");
        NullCheckTrap();

        Console.WriteLine("\n4. 值类型 vs 引用类型:");
        ValueTypeTrap();

        Console.WriteLine("\n5. 异步方法的同步上下文:");
        AsyncContextTrap();

        Console.WriteLine("\n6. LINQ 延迟执行陷阱:");
        LinqTrap();

        Console.WriteLine("\n7. 闭包和变量捕获:");
        ClosureTrap();

        Console.WriteLine("\n8. 类型转换陷阱:");
        TypeConversionTrap();
    }

    /// <summary>
    /// 陷阱 1: == vs .Equals()
    ///
    /// 【Java 对比】
    /// Java: == 比较引用，.equals() 比较值
    /// C#:   == 默认比较值（对于值类型），引用类型需要重载
    ///
    /// 【关键区别】
    /// C# 的 string 重载了 == 运算符，所以 == 比较的是值
    /// 但 object 类型的 == 比较的是引用
    /// </summary>
    private void EqualityTrap()
    {
        // 字符串比较
        string a = "Hello";
        string b = "Hello";
        string c = new string("Hello".ToCharArray());

        Console.WriteLine($"   a == b: {a == b}");           // true（值比较）
        Console.WriteLine($"   a == c: {a == c}");           // true（值比较）
        Console.WriteLine($"   ReferenceEquals(a, c): {ReferenceEquals(a, c)}"); // false（引用比较）

        // 对象比较
        object obj1 = 42;
        object obj2 = 42;

        Console.WriteLine($"   obj1 == obj2: {obj1 == obj2}");           // false（引用比较）
        Console.WriteLine($"   obj1.Equals(obj2): {obj1.Equals(obj2)}"); // true（值比较）

        // 整数比较（装箱陷阱）
        int x = 1000;
        int y = 1000;
        Console.WriteLine($"   x == y: {x == y}");           // true
        Console.WriteLine($"   (object)x == (object)y: {(object)x == (object)y}"); // false（装箱后比较引用）

        Console.WriteLine("   【建议】比较值类型时使用 == 或 .Equals()，比较引用类型时使用 .Equals()");
    }

    /// <summary>
    /// 陷阱 2: string 是引用类型但表现像值类型
    ///
    /// 【Java 对比】
    /// Java: String 是不可变的，但 == 比较引用
    /// C#:   string 是不可变的，== 比较值
    ///
    /// 【关键区别】
    /// C# 的 string 重载了 == 运算符，使其行为更直观
    /// </summary>
    private void StringTrap()
    {
        // 字符串驻留（String Interning）
        string s1 = "Hello";
        string s2 = "Hello";
        string s3 = new string("Hello".ToCharArray());

        Console.WriteLine($"   s1 == s2: {s1 == s2}");           // true
        Console.WriteLine($"   s1 == s3: {s1 == s3}");           // true
        Console.WriteLine($"   ReferenceEquals(s1, s2): {ReferenceEquals(s1, s2)}"); // true（驻留）
        Console.WriteLine($"   ReferenceEquals(s1, s3): {ReferenceEquals(s1, s3)}"); // false

        // 字符串拼接
        string result = "Hello" + " " + "World";
        Console.WriteLine($"   拼接结果: {result}");

        // StringBuilder vs string 拼接
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            sb.Append(i.ToString());
        }
        Console.WriteLine($"   StringBuilder 长度: {sb.Length}");

        Console.WriteLine("   【建议】大量字符串拼接使用 StringBuilder，少量拼接使用 + 即可");
    }

    /// <summary>
    /// 陷阱 3: null 检查的差异
    ///
    /// 【Java 对比】
    /// Java: null 检查使用 == null
    /// C#:   null 检查使用 is null（推荐）或 == null
    ///
    /// 【关键区别】
    /// C# 的 is null 不会被运算符重载影响，更安全
    /// </summary>
    private void NullCheckTrap()
    {
        string? name = null;

        // 推荐使用 is null
        if (name is null)
        {
            Console.WriteLine("   name is null: true");
        }

        // 也可以使用 == null，但可能被运算符重载影响
        if (name == null)
        {
            Console.WriteLine("   name == null: true");
        }

        // 空值条件运算符 ?.
        int? length = name?.Length;
        Console.WriteLine($"   name?.Length: {length}");

        // 空值合并运算符 ??
        string displayName = name ?? "默认名称";
        Console.WriteLine($"   name ?? \"默认名称\": {displayName}");

        // 空值合并赋值运算符 ??=
        name ??= "新名称";
        Console.WriteLine($"   name ??= \"新名称\": {name}");

        Console.WriteLine("   【建议】使用 is null 进行 null 检查，使用 ?. 和 ?? 处理可空类型");
    }

    /// <summary>
    /// 陷阱 4: 值类型 vs 引用类型
    ///
    /// 【Java 对比】
    /// Java: 基本类型（int, double）是值类型，对象是引用类型
    /// C#:   struct 是值类型，class 是引用类型
    ///
    /// 【关键区别】
    /// C# 的 struct 是值类型，赋值时会复制整个值
    /// </summary>
    private void ValueTypeTrap()
    {
        // 值类型赋值是复制
        var point1 = new StructPoint(10, 20);
        var point2 = point1; // 复制整个值
        point2.X = 30;

        Console.WriteLine($"   point1: ({point1.X}, {point1.Y})"); // (10, 20)
        Console.WriteLine($"   point2: ({point2.X}, {point2.Y})"); // (30, 20)

        // 引用类型赋值是复制引用
        var classPoint1 = new ClassPoint(10, 20);
        var classPoint2 = classPoint1; // 复制引用
        classPoint2.X = 30;

        Console.WriteLine($"   classPoint1: ({classPoint1.X}, {classPoint1.Y})"); // (30, 20)
        Console.WriteLine($"   classPoint2: ({classPoint2.X}, {classPoint2.Y})"); // (30, 20)

        // 可空值类型
        int? nullableInt = null;
        Console.WriteLine($"   nullableInt.HasValue: {nullableInt.HasValue}"); // false
        Console.WriteLine($"   nullableInt.GetValueOrDefault(): {nullableInt.GetValueOrDefault()}"); // 0

        Console.WriteLine("   【建议】理解值类型和引用类型的区别，避免意外的复制行为");
    }

    /// <summary>
    /// 陷阱 5: 异步方法的同步上下文
    ///
    /// 【Java 对比】
    /// Java: 没有同步上下文的概念
    /// C#:   await 会捕获同步上下文，可能导致死锁
    ///
    /// 【关键区别】
    /// 在 UI 线程或 ASP.NET 中，await 后续代码会在原线程执行
    /// 在控制台应用中，await 后续代码可能在不同线程执行
    /// </summary>
    private async void AsyncContextTrap()
    {
        // 在控制台应用中，没有同步上下文
        Console.WriteLine($"   主线程 ID: {Thread.CurrentThread.ManagedThreadId}");

        await Task.Delay(100);

        Console.WriteLine($"   await 后线程 ID: {Thread.CurrentThread.ManagedThreadId}");
        // 可能不同！因为控制台应用没有同步上下文

        // 【陷阱】在 UI 线程中使用 .Result 会导致死锁
        // 正确做法：使用 await
        // 错误做法：task.Result 或 task.Wait()

        Console.WriteLine("   【建议】");
        Console.WriteLine("   1. 在 UI 应用中，始终使用 await，不要使用 .Result 或 .Wait()");
        Console.WriteLine("   2. 在库方法中，使用 ConfigureAwait(false) 避免捕获同步上下文");
        Console.WriteLine("   3. async void 只用于事件处理器");
    }

    /// <summary>
    /// 陷阱 6: LINQ 延迟执行
    ///
    /// 【Java 对比】
    /// Java: Stream 也是延迟执行的
    /// C#:   LINQ 也是延迟执行的，但有一些细微差别
    ///
    /// 【关键区别】
    /// C# 的 IEnumerable 是延迟执行的，每次枚举都会重新计算
    /// </summary>
    private void LinqTrap()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        // 延迟执行：每次枚举都会重新计算
        var query = numbers.Where(n =>
        {
            Console.WriteLine($"   过滤 {n}");
            return n > 3;
        });

        Console.WriteLine("   第一次枚举:");
        foreach (var n in query)
        {
            Console.WriteLine($"   结果: {n}");
        }

        Console.WriteLine("   第二次枚举:");
        foreach (var n in query)
        {
            Console.WriteLine($"   结果: {n}");
        }

        // 【陷阱】多次枚举延迟执行的序列
        var filtered = numbers.Where(n => n > 3);
        Console.WriteLine($"   Count: {filtered.Count()}");     // 第一次枚举
        Console.WriteLine($"   Sum: {filtered.Sum()}");         // 第二次枚举！

        // 【解决方案】使用 ToList() 或 ToArray() 缓存结果
        var cached = numbers.Where(n => n > 3).ToList();
        Console.WriteLine($"   Cached Count: {cached.Count}");  // 不会重新计算
        Console.WriteLine($"   Cached Sum: {cached.Sum()}");    // 不会重新计算

        Console.WriteLine("   【建议】如果需要多次访问结果，使用 ToList() 或 ToArray() 缓存");
    }

    /// <summary>
    /// 陷阱 7: 闭包和变量捕获
    ///
    /// 【Java 对比】
    /// Java: Lambda 只能捕获 effectively final 变量
    /// C#:   Lambda 可以捕获并修改变量
    ///
    /// 【关键区别】
    /// C# 的闭包捕获的是变量本身，不是值
    /// </summary>
    private void ClosureTrap()
    {
        // 经典陷阱：循环变量捕获
        var actions = new List<Action>();

        for (int i = 0; i < 5; i++)
        {
            actions.Add(() => Console.WriteLine($"   i = {i}"));
        }

        Console.WriteLine("   执行所有 action:");
        foreach (var action in actions)
        {
            action(); // 都会打印 5，因为捕获的是变量 i 本身
        }

        // 【解决方案】使用局部变量
        var actions2 = new List<Action>();

        for (int i = 0; i < 5; i++)
        {
            int local = i; // 创建局部变量
            actions2.Add(() => Console.WriteLine($"   local = {local}"));
        }

        Console.WriteLine("   执行所有 action2:");
        foreach (var action in actions2)
        {
            action(); // 会打印 0, 1, 2, 3, 4
        }

        Console.WriteLine("   【建议】在循环中使用 Lambda 时，创建局部变量避免闭包陷阱");
    }

    /// <summary>
    /// 陷阱 8: 类型转换陷阱
    ///
    /// 【Java 对比】
    /// Java: (int) 3.14 会截断为 3
    /// C#:   (int) 3.14 也会截断为 3，但 Convert.ToInt32(3.14) 会四舍五入为 3
    ///
    /// 【关键区别】
    /// C# 有多种类型转换方式，行为不同
    /// </summary>
    private void TypeConversionTrap()
    {
        // 强制转换：截断
        double d = 3.7;
        int i1 = (int)d;
        Console.WriteLine($"   (int)3.7 = {i1}"); // 3

        // Convert: 四舍五入
        int i2 = Convert.ToInt32(d);
        Console.WriteLine($"   Convert.ToInt32(3.7) = {i2}"); // 4

        // Parse vs TryParse
        string numStr = "123";
        int parsed = int.Parse(numStr);
        Console.WriteLine($"   int.Parse(\"123\") = {parsed}");

        // TryParse: 安全的解析方式
        string invalidStr = "abc";
        if (int.TryParse(invalidStr, out int result))
        {
            Console.WriteLine($"   解析成功: {result}");
        }
        else
        {
            Console.WriteLine($"   解析失败: \"{invalidStr}\" 不是有效数字");
        }

        // 【陷阱】checked 和 unchecked
        try
        {
            checked
            {
                int max = int.MaxValue;
                int overflow = max + 1; // 会抛出 OverflowException
            }
        }
        catch (OverflowException)
        {
            Console.WriteLine("   checked 溢出检查: 捕获到 OverflowException");
        }

        unchecked
        {
            int max = int.MaxValue;
            int overflow = max + 1; // 不会抛出异常，会回绕
            Console.WriteLine($"   unchecked 溢出: {overflow}"); // -2147483648
        }

        Console.WriteLine("   【建议】");
        Console.WriteLine("   1. 使用 int.TryParse 安全解析字符串");
        Console.WriteLine("   2. 使用 checked 块检查数值溢出");
        Console.WriteLine("   3. 理解 (int) 和 Convert.ToInt32 的区别");
    }
}

// 辅助类型
public struct StructPoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public StructPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class ClassPoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public ClassPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}
