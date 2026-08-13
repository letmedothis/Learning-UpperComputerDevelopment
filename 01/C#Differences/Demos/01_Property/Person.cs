namespace C_Differences.Demos._01_Property;

/// <summary>
/// Person 类 - 展示 C# 自动属性（Auto-Implemented Properties）
///
/// 【Java 对比】
/// Java 中需要手写 getter/setter：
/// <code>
/// public class Person {
///     private String name = "";
///     private int age;
///
///     public String getName() { return name; }
///     public void setName(String name) { this.name = name; }
///     public int getAge() { return age; }
///     public void setAge(int age) { this.age = age; }
/// }
/// </code>
///
/// C# 用属性语法糖简化，编译器自动生成私有字段（backing field）
/// </summary>
public class Person
{
    /// <summary>
    /// 自动属性 - 最简写法
    /// 编译器会自动生成一个私有字段来存储值
    /// get; set; 表示可读可写
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 自动属性 - 值类型
    /// int 默认值为 0
    /// </summary>
    public int Age { get; set; }

    // 注意：如果需要在赋值时做验证，需要用"完整属性"（见 Account.cs）
}
