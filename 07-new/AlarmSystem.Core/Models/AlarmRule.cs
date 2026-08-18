namespace AlarmSystem.Core.Models;

/// <summary>
/// 报警规则。
/// </summary>
public sealed class AlarmRule
{
    // 名称和指标名都允许修改或包含分隔符，独立标识可避免它们拼接后产生冷却键碰撞。

    /// <summary>规则唯一标识。</summary>
    public string RuleId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>规则显示名称。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>监控的指标名称。</summary>
    public string MetricName { get; set; } = string.Empty;

    /// <summary>触发时的报警级别。</summary>
    public AlarmLevel Level { get; set; } = AlarmLevel.Warning;

    /// <summary>阈值。</summary>
    public double Threshold { get; set; }

    /// <summary>比较运算符：&gt;, &lt;, &gt;=, &lt;=, ==, !=</summary>
    public string Comparison { get; set; } = ">";

    /// <summary>报警消息模板，支持 {Metric}、{Threshold}、{Value} 占位符。</summary>
    public string MessageTemplate { get; set; } = "{Metric} 超过阈值 {Threshold}，当前值: {Value}";

    /// <summary>同一规则的冷却时间，避免重复报警。</summary>
    public TimeSpan Cooldown { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>是否启用该规则。</summary>
    public bool IsEnabled { get; set; } = true;
}
