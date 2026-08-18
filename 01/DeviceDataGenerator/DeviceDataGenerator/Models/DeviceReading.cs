namespace DeviceDataGenerator.Models;

/// <summary>
/// 设备读数记录 - 使用 record 实现不可变数据
///
/// 【为什么用 record】
/// 1. 不可变性：数据一旦创建就不能修改，线程安全
/// 2. 值相等性：两个相同数据的读数被认为是相等的
/// 3. with 表达式：可以基于现有数据创建副本并修改部分字段
/// 4. 解构：可以方便地提取各个字段
///
/// 【Java 对比】
/// Java 中需要 Lombok @Value 或手写不可变类：
/// <code>
/// @Value
/// public class DeviceReading {
///     LocalDateTime timestamp;
///     double temperature;
///     double pressure;
///     double speed;
/// }
/// </code>
/// </summary>
public record DeviceReading
{
    /// <summary>
    /// 时间戳 - 使用 init 访问器（UTC时间，避免时区歧义）
    /// init: 只能在对象初始化时赋值，之后不可修改
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// 温度 (°C)
    /// 正常范围: 15°C ~ 35°C
    /// </summary>
    public double Temperature { get; init; }

    /// <summary>
    /// 压力 (MPa)
    /// 正常范围: 0.1 MPa ~ 0.5 MPa
    /// </summary>
    public double Pressure { get; init; }

    /// <summary>
    /// 转速 (rpm)
    /// 正常范围: 1000 rpm ~ 2000 rpm
    /// </summary>
    public double Speed { get; init; }

    /// <summary>
    /// 格式化输出 - 方便日志显示
    /// </summary>
    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] 温度={Temperature:F1}°C 压力={Pressure:F3}MPa 转速={Speed:F0}rpm";
    }
}

/// <summary>
/// 传感器配置 - 使用 Property 实现可配置参数
///
/// 【为什么用 class + Property】
/// 1. 可变性：配置需要在运行时修改
/// 2. 验证逻辑：可以在 set 中添加验证（虽然这里没加）
/// 3. 绑定支持：可以与配置系统（IConfiguration）绑定
///
/// 【使用场景】
/// 通过配置文件或代码设置传感器的基准值和波动范围
/// </summary>
public class SensorConfig
{
    /// <summary>
    /// 温度基准值 (°C)
    /// 生成的温度会在此值上下波动
    /// </summary>
    public double TemperatureBase { get; set; } = 25.0;

    /// <summary>
    /// 温度波动范围 (°C)
    /// 实际温度 = TemperatureBase ± TemperatureRange/2
    /// </summary>
    public double TemperatureRange { get; set; } = 10.0;

    /// <summary>
    /// 压力基准值 (MPa)
    /// </summary>
    public double PressureBase { get; set; } = 0.3;

    /// <summary>
    /// 压力波动范围 (MPa)
    /// </summary>
    public double PressureRange { get; set; } = 0.2;

    /// <summary>
    /// 转速基准值 (rpm)
    /// </summary>
    public double SpeedBase { get; set; } = 1500;

    /// <summary>
    /// 转速波动范围 (rpm)
    /// </summary>
    public double SpeedRange { get; set; } = 500;
}

/// <summary>
/// 数据分析结果 - 使用 record 实现值对象
///
/// 【使用场景】
/// 存储 LINQ 分析的结果，包含最小值、最大值、平均值等统计信息
/// </summary>
public record AnalysisResult
{
    /// <summary>
    /// 指标名称（如"温度"、"压力"、"转速"）
    /// </summary>
    public string MetricName { get; init; } = string.Empty;

    /// <summary>最小值</summary>
    public double Min { get; init; }

    /// <summary>最大值</summary>
    public double Max { get; init; }

    /// <summary>平均值</summary>
    public double Average { get; init; }

    /// <summary>
    /// 样本数量
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// 格式化输出
    /// </summary>
    public override string ToString()
    {
        return $"{MetricName}: 最小={Min:F2}, 最大={Max:F2}, 平均={Average:F2} (共{Count}个样本)";
    }
}
