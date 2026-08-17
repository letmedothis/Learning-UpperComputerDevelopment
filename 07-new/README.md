## 报警系统 + 规则引擎

### 目标

实现可配置的报警规则引擎，支持多种比较操作、冷却时间和报警级别。

### 项目结构

```
07/
├── AlarmSystem.slnx
├── AlarmSystem.Core/
│   ├── Models/
│   │   ├── AlarmLevel.cs       # 报警级别枚举
│   │   ├── AlarmRecord.cs      # 报警记录
│   │   └── AlarmRule.cs        # 报警规则配置
│   └── Engine/
│       └── AlarmRuleEngine.cs  # 规则引擎
└── AlarmSystem.Tests/
    └── Engine/
        └── AlarmRuleEngineTests.cs
```

### 规则配置示例

```csharp
var rule = new AlarmRule
{
    Name = "温度过高",
    MetricName = "温度",
    Threshold = 40,
    Comparison = ">",      // >, <, >=, <=, ==, !=
    Level = AlarmLevel.Alarm,
    Cooldown = TimeSpan.FromMinutes(1),
    MessageTemplate = "{Metric} 超过阈值 {Threshold}，当前值: {Value}"
};
```

### 功能特性

- 支持 6 种比较操作（>, <, >=, <=, ==, !=）
- 冷却时间防止重复报警
- 4 级报警级别（Info, Warning, Alarm, Critical）
- 批量规则评估
- 消息模板变量替换

### 运行命令

```bash
dotnet build ./07/AlarmSystem.slnx -c Release
dotnet test ./07/AlarmSystem.slnx -c Release
```
