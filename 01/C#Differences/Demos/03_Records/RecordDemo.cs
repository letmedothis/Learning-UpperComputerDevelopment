namespace C_Differences.Demos._03_Records;

/// <summary>
/// record 类型演示
/// </summary>
public class RecordDemo
{
    public void Demo()
    {
        Console.WriteLine("1. 基本 record 类型:");
        var point1 = new Point(3, 4);
        var point2 = new Point(3, 4);
        var point3 = new Point(5, 6);

        Console.WriteLine($"   point1: {point1}");
        Console.WriteLine($"   point2: {point2}");
        Console.WriteLine($"   point3: {point3}");

        // 值相等性 - record 默认基于值比较
        Console.WriteLine($"\n2. 值相等性:");
        Console.WriteLine($"   point1 == point2: {point1 == point2}"); // true
        Console.WriteLine($"   point1 == point3: {point1 == point3}"); // false
        Console.WriteLine($"   point1.Equals(point2): {point1.Equals(point2)}"); // true

        // 引用比较
        Console.WriteLine($"   ReferenceEquals(point1, point2): {ReferenceEquals(point1, point2)}"); // false

        // 不可变性 - with 表达式创建副本
        Console.WriteLine($"\n3. 不可变性与 with 表达式:");
        var point4 = point1 with { X = 10 };
        Console.WriteLine($"   point1: {point1}");
        Console.WriteLine($"   point4 (point1 with X=10): {point4}");

        // 解构
        Console.WriteLine($"\n4. 解构:");
        var (x, y) = point1;
        Console.WriteLine($"   解构 point1: X={x}, Y={y}");

        // 实际应用：DTO 和值对象
        Console.WriteLine($"\n5. 实际应用 - DTO:");
        var order = new Order(1, "张三", new DateTime(2024, 1, 15));
        Console.WriteLine($"   订单: {order}");

        // 继承
        Console.WriteLine($"\n6. record 继承:");
        var coloredPoint = new ColoredPoint(1, 2, "红色");
        Console.WriteLine($"   彩色点: {coloredPoint}");

        // 位置参数与属性混合
        Console.WriteLine($"\n7. 位置参数与属性混合:");
        var person = new PersonRecord("王五", 30) { Email = "wangwu@example.com" };
        Console.WriteLine($"   人员: {person}");
        Console.WriteLine($"   邮箱: {person.Email}");
    }
}

// 基本 record 类型
public record Point(int X, int Y);

// 带默认值的 record
public record Order(int Id, string Customer, DateTime OrderDate);

// record 继承
public record ColoredPoint(int X, int Y, string Color) : Point(X, Y);

// 位置参数与属性混合
public record PersonRecord(string Name, int Age)
{
    public string? Email { get; init; }
}

// record struct (C# 10+)
public readonly record struct Vector(double X, double Y, double Z);
