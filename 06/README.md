## 历史数据趋势图

### 目标

使用 WPF Canvas 绘制历史数据趋势图，展示温度、压力、转速的变化趋势和统计信息。

### 项目结构

```
06/
├── TrendChart.slnx
├── TrendChart.Core/
│   ├── Models/
│   │   ├── TrendDataPoint.cs     # 数据点
│   │   ├── TrendSeries.cs        # 数据系列
│   │   └── TrendStatistics.cs    # 统计结果
│   └── Services/
│       └── TrendDataGenerator.cs # 数据生成 + 统计计算
├── TrendChart.App/
│   ├── MainWindow.xaml           # 趋势图界面
│   └── MainWindow.xaml.cs        # Canvas 绘图逻辑
└── TrendChart.Tests/
```

### 功能特性

- 温度、压力、转速三条趋势线
- 自适应 Canvas 大小
- 网格线和坐标轴
- 图例显示
- 统计信息（最小/最大/平均/标准差）

### 运行命令

```bash
dotnet build ./06/TrendChart.slnx -c Release
dotnet test ./06/TrendChart.slnx -c Release
dotnet run --project ./06/TrendChart.App/TrendChart.App.csproj
```
