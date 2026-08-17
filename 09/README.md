## 集成测试 + 部署打包

### 目标

构建集成测试框架和部署配置生成器，支持多步骤测试和自动化部署。

### 项目结构

```
09/
├── IntegrationTest.slnx
├── IntegrationTest.Core/
│   ├── TestRunner.cs          # 集成测试运行器
│   ├── TestResult.cs          # 测试结果模型
│   └── DeploymentConfig.cs    # 部署配置生成器
└── IntegrationTest.Tests/
    ├── TestRunnerTests.cs
    └── DeploymentConfigTests.cs
```

### 集成测试运行器

```csharp
var runner = new TestRunner()
    .AddSetup(async () => { /* 初始化数据库 */ })
    .AddTest(async () => { /* 测试 API */ return true; })
    .AddTest(async () => { /* 测试数据库 */ return true; })
    .AddCleanup(async () => { /* 清理资源 */ });

var result = await runner.RunAsync();
// result.IsSuccess, result.TestsPassed, result.TestsFailed
```

### 部署配置

```csharp
var config = new DeploymentConfig
{
    ApplicationName = "UpperComputerMonitor",
    Version = "1.0.0",
    RuntimeIdentifier = "win-x64",
    SelfContained = true
};

// 生成发布命令
var command = config.GeneratePublishCommand("./MyApp.csproj");

// 生成 Dockerfile
var dockerfile = config.GenerateDockerfile("./MyApp.csproj");
```

### 运行命令

```bash
dotnet build ./09/IntegrationTest.slnx -c Release
dotnet test ./09/IntegrationTest.slnx -c Release
```

### 学习总结

9 周学习路径完成：

| 周次 | 主题 | 核心技能 |
|------|------|----------|
| 01 | C# 基础 | 属性、record、LINQ、async/await |
| 02 | WPF V1 | XAML、Binding、Dispatcher |
| 03 | MVVM | ViewModel、ICommand、INotifyPropertyChanged |
| 04 | Modbus 通信 | 接口抽象、异步通信 |
| 05 | 数据持久化 | EF Core、SQLite、仓储模式 |
| 06 | 趋势图 | Canvas 绘图、数据分析 |
| 07 | 报警系统 | 规则引擎、条件评估 |
| 08 | Web API | REST API、CORS、依赖注入 |
| 09 | 集成测试 | 测试框架、部署配置 |
