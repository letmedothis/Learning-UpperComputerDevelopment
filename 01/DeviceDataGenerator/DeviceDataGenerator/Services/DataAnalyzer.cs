using DeviceDataGenerator.Models;

namespace DeviceDataGenerator.Services;

/// <summary>
/// 数据分析器 - 使用 LINQ 进行数据分析
///
/// 【综合应用的知识点】
/// 1. LINQ: 使用 Where、Select、Min、Max、Average、GroupBy 等操作
/// 2. 泛型集合: List&lt;T&gt;、IEnumerable&lt;T&gt;、IReadOnlyList&lt;T&gt;
/// 3. 表达式体成员: =&gt; 简化单行方法
/// 4. 元组: (string Range, int Count) 返回多个值
///
/// 【滑动窗口模式】
/// 保持最近 N 个样本，超出时移除最旧的数据
/// 适合实时数据分析场景
/// </summary>
public class DataAnalyzer
{
    // ========== 私有字段 ==========

    /// <summary>
    /// 数据存储 - 使用 List 保存最近的读数
    /// </summary>
    private readonly List<DeviceReading> _readings = new();

    /// <summary>
    /// 最大样本数量 - 滑动窗口大小
    /// 默认保存最近 60 个样本（1分钟的数据，每秒1条）
    /// </summary>
    private readonly int _maxSamples;

    // ========== 构造函数 ==========

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="maxSamples">最大样本数量，默认60</param>
    public DataAnalyzer(int maxSamples = 60)
    {
        // 滑动窗口必须至少容纳一个样本，否则 AddReading 会不断删除，
        // 负数容量还会在列表已空时继续 RemoveAt(0)。
        ArgumentOutOfRangeException.ThrowIfLessThan(maxSamples, 1);
        _maxSamples = maxSamples;
    }

    // ========== 公共属性 ==========

    /// <summary>
    /// 当前样本数量
    /// 使用表达式体属性 =&gt; 简化只读属性
    /// </summary>
    public int SampleCount => _readings.Count;

    // ========== 公共方法 ==========

    /// <summary>
    /// 添加新的读数
    ///
    /// 【滑动窗口逻辑】
    /// 当数据超过最大数量时，移除最旧的数据（索引0）
    /// 保证只保留最近的 N 条数据
    /// </summary>
    /// <param name="reading">新的读数</param>
    public void AddReading(DeviceReading reading)
    {
        _readings.Add(reading);

        // 保持最近 N 个样本，移除最旧的
        while (_readings.Count > _maxSamples)
        {
            _readings.RemoveAt(0); // 移除第一个（最旧的）
        }
    }

    /// <summary>
    /// 批量添加读数
    /// </summary>
    /// <param name="readings">读数集合</param>
    public void AddReadings(IEnumerable<DeviceReading> readings)
    {
        foreach (var reading in readings)
        {
            AddReading(reading);
        }
    }

    /// <summary>
    /// 获取所有读数（只读）
    ///
    /// 【为什么返回 IReadOnlyList】
    /// 防止外部代码修改内部数据，遵循封装原则
    /// </summary>
    /// <returns>只读的读数列表</returns>
    public IReadOnlyList<DeviceReading> GetReadings()
    {
        return _readings.AsReadOnly();
    }

    /// <summary>
    /// 分析温度数据 - 使用 LINQ
    ///
    /// 【LINQ 操作说明】
    /// Select: 投影，从 DeviceReading 中提取 Temperature 字段
    /// </summary>
    /// <returns>温度分析结果</returns>
    public AnalysisResult AnalyzeTemperature()
    {
        // _readings.Select(r =&gt; r.Temperature) 提取所有温度值
        return AnalyzeMetric("温度", _readings.Select(r => r.Temperature));
    }

    /// <summary>
    /// 分析压力数据 - 使用 LINQ
    /// </summary>
    /// <returns>压力分析结果</returns>
    public AnalysisResult AnalyzePressure()
    {
        return AnalyzeMetric("压力", _readings.Select(r => r.Pressure));
    }

    /// <summary>
    /// 分析转速数据 - 使用 LINQ
    /// </summary>
    /// <returns>转速分析结果</returns>
    public AnalysisResult AnalyzeSpeed()
    {
        return AnalyzeMetric("转速", _readings.Select(r => r.Speed));
    }

    /// <summary>
    /// 获取最近 N 条读数
    ///
    /// 【LINQ 操作说明】
    /// TakeLast(n): 取最后 n 个元素
    /// </summary>
    /// <param name="count">要获取的数量</param>
    /// <returns>最近的读数</returns>
    public IEnumerable<DeviceReading> GetRecentReadings(int count)
    {
        return _readings.TakeLast(count);
    }

    /// <summary>
    /// 查找温度超限的读数
    ///
    /// 【LINQ 操作说明】
    /// Where: 过滤，只保留满足条件的元素
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>超限的读数</returns>
    public IEnumerable<DeviceReading> FindTemperatureOutOfRange(double min, double max)
    {
        // Where: 过滤条件，保留温度超出范围的读数
        return _readings.Where(r => r.Temperature < min || r.Temperature > max);
    }

    /// <summary>
    /// 按温度范围分组统计
    ///
    /// 【LINQ 操作说明】
    /// 1. GroupBy: 按指定条件分组
    /// 2. OrderBy: 排序
    /// 3. Select: 投影转换
    ///
    /// 【分组算法】
    /// Math.Floor(temperature / step) * step
    /// 例如 step=5: 23.5 → 20, 27.8 → 25, 31.2 → 30
    /// </summary>
    /// <param name="step">分组步长，默认5°C</param>
    /// <returns>分组统计结果</returns>
    public IEnumerable<(string Range, int Count)> GroupByTemperatureRange(double step = 5.0)
    {
        // 分组步长是除数，也是下一段区间的增量；零或负数没有可解释的区间语义。
        if (step <= 0 || double.IsNaN(step) || double.IsInfinity(step))
            throw new ArgumentOutOfRangeException(nameof(step), step, "温度分组步长必须是有限的正数。");

        // 空数据检查
        if (!_readings.Any())
            return Enumerable.Empty<(string, int)>();

        // GroupBy: 按温度范围分组
        // OrderBy: 按分组键（温度范围起点）排序
        // Select: 转换为 (范围描述, 数量) 元组
        return _readings
            .GroupBy(r => Math.Floor(r.Temperature / step) * step) // 分组键：温度范围起点
            .OrderBy(g => g.Key)                                   // 按范围起点排序
            .Select(g => ($"{g.Key:F0}-{g.Key + step:F0}°C", g.Count())); // 转换格式
    }

    // ========== 私有方法 ==========

    /// <summary>
    /// 通用分析方法 - 演示 LINQ 的聚合操作
    ///
    /// 【LINQ 聚合操作】
    /// Min(): 最小值
    /// Max(): 最大值
    /// Average(): 平均值
    /// Count(): 数量
    /// Any(): 是否有元素
    /// </summary>
    /// <param name="name">指标名称</param>
    /// <param name="values">数值序列</param>
    /// <returns>分析结果</returns>
    private AnalysisResult AnalyzeMetric(string name, IEnumerable<double> values)
    {
        // Any() 检查是否有数据，避免空序列的聚合操作
        if (!values.Any())
        {
            return new AnalysisResult
            {
                MetricName = name,
                Min = 0,
                Max = 0,
                Average = 0,
                Count = 0
            };
        }

        // 使用 LINQ 聚合操作计算统计值
        return new AnalysisResult
        {
            MetricName = name,
            Min = values.Min(),           // 最小值
            Max = values.Max(),           // 最大值
            Average = values.Average(),   // 平均值
            Count = values.Count()        // 数量
        };
    }

    /// <summary>
    /// 清空所有数据
    /// </summary>
    public void Clear()
    {
        _readings.Clear();
    }
}
