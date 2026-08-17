## WPF 设备监控 V1

### 目标

使用 WPF 构建第一版设备监控界面，每秒展示设备状态、温度、压力、转速、累计产量以及最近 20 条采样记录。

### 项目结构

```
02/
├── ProductionLineMonitor.slnx
├── ProductionLineMonitor.Core/          ← 纯 C# 核心逻辑（无 WPF 依赖）
│   ├── Models/
│   │   ├── MetricLevel.cs              # 状态等级枚举
│   │   └── DeviceReading.cs            # 不可变采样记录
│   └── Services/
│       ├── ReadingEvaluator.cs         # 阈值判断（纯函数）
│       ├── RecentReadingBuffer.cs      # 最近 20 条缓冲区
│       ├── FakeDataGenerator.cs        # 模拟数据生成
│       └── AcquisitionService.cs       # 后台定时采集
├── ProductionLineMonitor.App/          ← WPF 界面
│   ├── Models/
│   │   ├── OperatingState.cs           # 运行状态枚举
│   │   ├── MetricCardItem.cs           # 指标卡片绑定项
│   │   └── DashboardState.cs           # 仪表盘快照
│   ├── Converters/
│   │   ├── MetricLevelToBrushConverter.cs
│   │   └── OperatingStateToBrushConverter.cs
│   ├── Themes/
│   │   ├── Colors.xaml                 # 颜色资源
│   │   └── Controls.xaml               # 控件样式
│   ├── App.xaml                        # 合并资源字典
│   ├── MainWindow.xaml                 # 主界面布局
│   └── MainWindow.xaml.cs              # 生命周期管理
├── ProductionLineMonitor.Tests/        ← 单元测试
│   └── Services/
│       ├── ReadingEvaluatorTests.cs
│       ├── RecentReadingBufferTests.cs
│       ├── FakeDataGeneratorTests.cs
│       └── AcquisitionServiceTests.cs
└── README.md
```

### 运行命令

```bash
# 构建
dotnet build ./02/ProductionLineMonitor.slnx -c Release

# 运行测试
dotnet test ./02/ProductionLineMonitor.slnx -c Release

# 启动程序
dotnet run --project ./02/ProductionLineMonitor.App/ProductionLineMonitor.App.csproj
```

### Java vs C# 对照

| 概念 | Java | C# |
|------|------|-----|
| 不可变数据 | `record` / Lombok `@Value` | `record` |
| 线程切换 | `SwingUtilities.invokeLater` | `Dispatcher.InvokeAsync` |
| 取消机制 | `volatile boolean` | `CancellationToken` |
| 后台任务 | `ScheduledExecutorService` | `Task.Delay` + `async/await` |
| 数据绑定 | 手动更新控件 | `DataContext` + XAML `Binding` |

### 状态阈值

| 指标 | 正常 | 警告 | 报警 |
|------|------|------|------|
| 温度 | 15–35 °C | 10–15 / 35–40 | <10 / >40 |
| 压力 | 0.1–0.5 MPa | 0.05–0.1 / 0.5–0.6 | <0.05 / >0.6 |
| 转速 | 1000–2000 rpm | 500–1000 / 2000–2500 | <500 / >2500 |

### V2 重构方向

- 引入 MVVM 模式（ViewModel + ICommand + INotifyPropertyChanged）
- 使用依赖注入容器管理服务
- 接入真实设备通信（Modbus TCP / OPC UA）
- 添加数据持久化（SQLite + EF Core）
- 历史数据趋势图
