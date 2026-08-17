namespace AlarmSystem.Core.Models;

/// <summary>
/// 报警规则。
/// </summary>
public sealed class AlarmRule
{
    // 名称和指标名都允许修改或包含分隔符，独立标识可避免它们拼接后产生冷却键碰撞。
    public string RuleId { get; init; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public AlarmLevel Level { get; set; } = AlarmLevel.Warning;
    public double Threshold { get; set; }
    public string Comparison { get; set; } = ">"; // >, <, >=, <=, ==, !=
    public string MessageTemplate { get; set; } = "{Metric} 超过阈值 {Threshold}，当前值: {Value}";
    public TimeSpan Cooldown { get; set; } = TimeSpan.FromMinutes(1);
    public bool IsEnabled { get; set; } = true;
}
