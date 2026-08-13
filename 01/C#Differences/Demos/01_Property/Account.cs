namespace C_Differences.Demos._01_Property;

/// <summary>
/// Account 类 - 展示带验证的属性（Full Properties with Validation）
///
/// 【Java 对比】
/// Java 中在 setter 中做验证：
/// <code>
/// public class Account {
///     private BigDecimal balance;
///
///     public void setBalance(BigDecimal balance) {
///         if (balance.compareTo(BigDecimal.ZERO) < 0) {
///             throw new IllegalArgumentException("余额不能为负数");
///         }
///         this.balance = balance;
///     }
/// }
/// </code>
///
/// C# 在属性的 set 访问器中写验证逻辑
/// </summary>
public class Account
{
    /// <summary>
    /// 私有字段 - 存储实际的余额值
    /// 命名约定：下划线开头 + 驼峰命名
    /// </summary>
    private decimal _balance;

    /// <summary>
    /// 带验证的属性 - 完整写法
    ///
    /// 与自动属性不同，这里需要手写 get/set 的实现
    /// - get: 使用表达式体语法 =&gt; 直接返回字段
    /// - set: 使用 value 关键字获取赋值，可添加验证逻辑
    ///
    /// 【关键语法】
    /// - value: C# 关键字，在 set 访问器中表示要赋的值
    /// - get =&gt; 表达式: 简化写法，等价于 get { return _balance; }
    /// </summary>
    public decimal Balance
    {
        get => _balance;
        set
        {
            // 在 set 中添加业务逻辑验证
            if (value < 0)
                throw new ArgumentException("余额不能为负数");

            // 验证通过后才赋值
            _balance = value;
        }
    }

    // 使用示例：
    // var account = new Account();
    // account.Balance = 1000;     // ✅ 正常赋值
    // account.Balance = -100;     // ❌ 抛出 ArgumentException
}
