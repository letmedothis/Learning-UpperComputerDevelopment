namespace C_Differences.Demos._01_Property;

/// <summary>
/// Product 类 - 展示只读属性（Read-Only Properties）
///
/// 【Java 对比】
/// Java 中用 final 字段 + getter 实现只读：
/// <code>
/// public class Product {
///     private final String name;
///     private final BigDecimal price;
///
///     public Product(String name, BigDecimal price) {
///         this.name = name;
///         this.price = price;
///     }
///
///     public String getName() { return name; }
///     public BigDecimal getPrice() { return price; }
///     // 没有 setter 方法
/// }
/// </code>
///
/// C# 只需省略 set 即可，更简洁
/// </summary>
public class Product
{
    /// <summary>
    /// 只读属性 - 只有 get，没有 set
    /// 只能在构造函数中赋值，之后不可修改
    /// 适合创建不可变对象（Immutable Object）
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 只读属性 - 价格
    /// 使用 decimal 类型避免浮点精度问题（适合金额计算）
    /// </summary>
    public decimal Price { get; }

    /// <summary>
    /// 构造函数 - 唯一可以设置只读属性的地方
    /// </summary>
    /// <param name="name">产品名称</param>
    /// <param name="price">产品价格</param>
    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    // 注意：如果尝试在构造函数外赋值，编译会报错：
    // product.Name = "新名称"; // ❌ 编译错误：属性或索引器"Name"无法赋值，因为它是只读的
}
