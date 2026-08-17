using AlarmSystem.Core.Models;

namespace AlarmSystem.Core.Engine;

/// <summary>
/// 报警规则引擎 —— 评估规则并生成报警记录。
/// </summary>
public sealed class AlarmRuleEngine
{
    private readonly List<AlarmRule> _rules = new();
    private readonly Dictionary<string, DateTime> _lastAlarmTime = new();

    /// <summary>
    /// 添加报警规则。
    /// </summary>
    public void AddRule(AlarmRule rule)
    {
        _rules.Add(rule);
    }

    /// <summary>
    /// 批量添加规则。
    /// </summary>
    public void AddRules(IEnumerable<AlarmRule> rules)
    {
        _rules.AddRange(rules);
    }

    /// <summary>
    /// 获取所有规则。
    /// </summary>
    public IReadOnlyList<AlarmRule> GetRules() => _rules.AsReadOnly();

    /// <summary>
    /// 评估所有规则，返回触发的报警。
    /// </summary>
    public List<AlarmRecord> Evaluate(Dictionary<string, double> metrics)
    {
        var alarms = new List<AlarmRecord>();
        var now = DateTime.Now;

        foreach (var rule in _rules.Where(r => r.IsEnabled))
        {
            if (!metrics.TryGetValue(rule.MetricName, out var value))
                continue;

            if (!EvaluateCondition(value, rule.Threshold, rule.Comparison))
                continue;

            // 冷却状态必须跟随稳定规则标识，不能依赖可修改且可能拼接碰撞的显示名称。
            var key = rule.RuleId;
            if (_lastAlarmTime.TryGetValue(key, out var lastTime))
            {
                if (now - lastTime < rule.Cooldown)
                    continue;
            }

            var message = FormatMessage(rule, value);
            var alarm = new AlarmRecord(
                Id: Guid.NewGuid().ToString("N")[..8],
                RuleName: rule.Name,
                MetricName: rule.MetricName,
                Value: value,
                Threshold: rule.Threshold,
                Level: rule.Level,
                Message: message,
                Timestamp: now);

            alarms.Add(alarm);
            _lastAlarmTime[key] = now;
        }

        return alarms;
    }

    /// <summary>
    /// 清除冷却时间（用于测试或重置）。
    /// </summary>
    public void ClearCooldowns()
    {
        _lastAlarmTime.Clear();
    }

    private static bool EvaluateCondition(double value, double threshold, string comparison)
    {
        return comparison switch
        {
            ">" => value > threshold,
            "<" => value < threshold,
            ">=" => value >= threshold,
            "<=" => value <= threshold,
            "==" => Math.Abs(value - threshold) < 0.0001,
            "!=" => Math.Abs(value - threshold) >= 0.0001,
            // 配置错误不能伪装成“条件未触发”，否则现场会把漏报误判为正常值。
            _ => throw new ArgumentException($"不支持的比较符: {comparison}", nameof(comparison))
        };
    }

    private static string FormatMessage(AlarmRule rule, double value)
    {
        return rule.MessageTemplate
            .Replace("{Metric}", rule.MetricName)
            .Replace("{Threshold}", rule.Threshold.ToString("F2"))
            .Replace("{Value}", value.ToString("F2"));
    }
}
