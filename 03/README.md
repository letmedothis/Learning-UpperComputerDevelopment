## WPF 设备监控 V3 (MVVM 重构)

### 目标

将 V1 的"替换 DataContext 快照"重构为标准 MVVM 模式：ViewModel + ICommand + INotifyPropertyChanged + ObservableCollection。

### 核心变化

| V1 (Week 02) | V3 (Week 03) |
|--------------|--------------|
| 整体替换 `DataContext` | 单个属性独立通知 |
| `Click` 事件处理 | `ICommand` 绑定 |
| 代码后置逻辑 | `ViewModel` 分离 |
| `List` 手动清空 | `ObservableCollection` 自动更新 |

### 项目结构

```
03/
├── MvvmMonitor.slnx
├── MvvmMonitor.Core/
│   ├── Models/
│   │   ├── MetricLevel.cs
│   │   └── DeviceReading.cs
│   ├── Services/
│   │   ├── ReadingEvaluator.cs
│   │   ├── FakeDataGenerator.cs
│   │   └── AcquisitionService.cs
│   └── Mvvm/
│       ├── ViewModelBase.cs      ← INotifyPropertyChanged 基类
│       └── RelayCommand.cs       ← ICommand 实现
├── MvvmMonitor.App/
│   ├── ViewModels/
│   │   ├── MainViewModel.cs      ← 所有 UI 逻辑
│   │   └── MetricCardViewModel.cs
│   ├── Converters/
│   ├── Themes/
│   ├── App.xaml
│   ├── MainWindow.xaml           ← 只有 XAML 绑定
│   └── MainWindow.xaml.cs        ← 只有 InitializeComponent
└── MvvmMonitor.Tests/
    ├── Services/
    └── ViewModels/
```

### Java vs C# MVVM 对照

| 概念 | Java | C# |
|------|------|-----|
| 属性通知 | JavaFX `Observable` | `INotifyPropertyChanged` |
| 命令绑定 | `ActionListener` | `ICommand` / `RelayCommand` |
| 集合通知 | `ObservableList` | `ObservableCollection<T>` |
| 数据绑定 | 手动更新控件 | XAML `{Binding}` |

### 运行命令

```bash
dotnet build ./03/MvvmMonitor.slnx -c Release
dotnet test ./03/MvvmMonitor.slnx -c Release
dotnet run --project ./03/MvvmMonitor.App/MvvmMonitor.App.csproj
```
