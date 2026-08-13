namespace C_Differences.Demos._02_NullableReference;

/// <summary>
/// 可空引用类型演示
/// </summary>
public class NullableReferenceDemo
{
    // 非空引用类型 - 必须初始化或赋值
    public string NonNullableName { get; set; } = string.Empty;

    // 可空引用类型 - 可以为 null
    public string? NullableName { get; set; }

    // 可空值类型
    public int? NullableAge { get; set; }

    public void Demo()
    {
        Console.WriteLine("1. 非空引用类型:");
        Console.WriteLine($"   NonNullableName: '{NonNullableName}' (已初始化为空字符串)");

        Console.WriteLine("\n2. 可空引用类型:");
        Console.WriteLine($"   NullableName: {(NullableName == null ? "null" : NullableName)}");

        Console.WriteLine("\n3. 可空值类型:");
        Console.WriteLine($"   NullableAge: {(NullableAge.HasValue ? NullableAge.Value.ToString() : "null")}");

        // 安全地处理可空类型
        Console.WriteLine("\n4. 安全处理可空类型:");

        // 使用空值合并运算符 ??
        string displayName = NullableName ?? "默认名称";
        Console.WriteLine($"   使用 ?? 运算符: {displayName}");

        // 使用空值条件运算符 ?.
        int? length = NullableName?.Length;
        Console.WriteLine($"   使用 ?. 运算符: 长度 = {(length.HasValue ? length.Value.ToString() : "null")}");

        // 模式匹配检查 null
        if (NullableName is not null)
        {
            Console.WriteLine($"   模式匹配: NullableName 不为 null");
        }
        else
        {
            Console.WriteLine($"   模式匹配: NullableName 为 null");
        }

        // 实际应用示例
        Console.WriteLine("\n5. 实际应用示例:");
        var user = new User { Name = "李四" };
        Console.WriteLine($"   用户: {user.Name}");
        Console.WriteLine($"   昵称: {user.Nickname ?? "未设置"}");
        Console.WriteLine($"   显示名称: {user.GetDisplayName()}");
    }
}

/// <summary>
/// User 类 - 可空引用类型实际应用
/// </summary>
public class User
{
    public string Name { get; set; } = string.Empty;
    public string? Nickname { get; set; }

    public string GetDisplayName()
    {
        // 使用空值合并运算符
        return Nickname ?? Name;
    }
}
