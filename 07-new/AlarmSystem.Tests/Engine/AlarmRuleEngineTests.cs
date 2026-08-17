using AlarmSystem.Core.Engine;
using AlarmSystem.Core.Models;

namespace AlarmSystem.Tests.Engine;

public sealed class AlarmRuleEngineTests
{
    [Fact]
    public void Evaluate_WhenValueExceedsThreshold_ReturnsAlarm()
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule
        {
            Name = "温度过高",
            MetricName = "温度",
            Threshold = 40,
            Comparison = ">",
            Level = AlarmLevel.Alarm
        });

        var metrics = new Dictionary<string, double> { ["温度"] = 45 };
        var alarms = engine.Evaluate(metrics);

        Assert.Single(alarms);
        Assert.Equal("温度过高", alarms[0].RuleName);
        Assert.Equal(45, alarms[0].Value);
    }

    [Fact]
    public void Evaluate_WhenValueBelowThreshold_ReturnsNoAlarm()
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule
        {
            Name = "温度过高",
            MetricName = "温度",
            Threshold = 40,
            Comparison = ">"
        });

        var metrics = new Dictionary<string, double> { ["温度"] = 35 };
        var alarms = engine.Evaluate(metrics);

        Assert.Empty(alarms);
    }

    [Fact]
    public void Evaluate_WithCooldown_SuppressesRepeatedAlarms()
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule
        {
            Name = "温度过高",
            MetricName = "温度",
            Threshold = 40,
            Comparison = ">",
            Cooldown = TimeSpan.FromMinutes(5)
        });

        var metrics = new Dictionary<string, double> { ["温度"] = 45 };

        var alarms1 = engine.Evaluate(metrics);
        var alarms2 = engine.Evaluate(metrics);

        Assert.Single(alarms1);
        Assert.Empty(alarms2); // 被冷却时间抑制
    }

    [Fact]
    public void Evaluate_WithMultipleRules_EvaluatesAll()
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule { Name = "温度高", MetricName = "温度", Threshold = 40, Comparison = ">" });
        engine.AddRule(new AlarmRule { Name = "压力低", MetricName = "压力", Threshold = 0.1, Comparison = "<" });

        var metrics = new Dictionary<string, double>
        {
            ["温度"] = 45,
            ["压力"] = 0.05
        };

        var alarms = engine.Evaluate(metrics);
        Assert.Equal(2, alarms.Count);
    }

    [Fact]
    public void Evaluate_WithCollidingNames_UsesRuleIdForIndependentCooldowns()
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule
        {
            RuleId = "rule-one",
            Name = "A_B",
            MetricName = "C",
            Threshold = 10,
            Comparison = ">"
        });
        engine.AddRule(new AlarmRule
        {
            RuleId = "rule-two",
            Name = "A",
            MetricName = "B_C",
            Threshold = 10,
            Comparison = ">"
        });

        var alarms = engine.Evaluate(new Dictionary<string, double>
        {
            ["C"] = 11,
            ["B_C"] = 11
        });

        Assert.Equal(2, alarms.Count);
    }

    [Fact]
    public void Evaluate_WithLessThanComparison_WorksCorrectly()
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule
        {
            Name = "压力过低",
            MetricName = "压力",
            Threshold = 0.1,
            Comparison = "<",
            Level = AlarmLevel.Warning
        });

        var metrics = new Dictionary<string, double> { ["压力"] = 0.05 };
        var alarms = engine.Evaluate(metrics);

        Assert.Single(alarms);
        Assert.Equal(AlarmLevel.Warning, alarms[0].Level);
    }

    [Theory]
    [InlineData(">", 11, true)]
    [InlineData(">", 10, false)]
    [InlineData("<", 9, true)]
    [InlineData("<", 10, false)]
    [InlineData(">=", 10, true)]
    [InlineData(">=", 9, false)]
    [InlineData("<=", 10, true)]
    [InlineData("<=", 11, false)]
    [InlineData("==", 10, true)]
    [InlineData("==", 10.001, false)]
    [InlineData("!=", 10.001, true)]
    [InlineData("!=", 10, false)]
    public void Evaluate_WithSupportedComparison_ReturnsExpectedResult(
        string comparison,
        double value,
        bool shouldTrigger)
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule
        {
            Name = "边界比较",
            MetricName = "测量值",
            Threshold = 10,
            Comparison = comparison
        });

        var alarms = engine.Evaluate(new Dictionary<string, double> { ["测量值"] = value });

        Assert.Equal(shouldTrigger, alarms.Count == 1);
    }

    [Fact]
    public void Evaluate_WithUnknownComparison_ThrowsArgumentException()
    {
        var engine = new AlarmRuleEngine();
        engine.AddRule(new AlarmRule
        {
            Name = "错误配置",
            MetricName = "温度",
            Threshold = 40,
            Comparison = "=>"
        });

        var exception = Assert.Throws<ArgumentException>(() =>
            engine.Evaluate(new Dictionary<string, double> { ["温度"] = 45 }));

        Assert.Contains("=>", exception.Message);
    }
}
