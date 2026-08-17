# WPF 设备监控 V1 设计

## 1. 背景与目标

第 2 周的目标是在 `02` 目录内完成第一版 WPF 设备监控程序。程序使用假数据，每秒展示设备状态、温度、压力、转速、累计产量以及最近 20 条采样记录。

本版本面向从 Java 转向 C# 的学习者，代码需要职责单一、注释清楚、容易调试。V1 采用“分层服务 + 少量窗口代码后置”，不提前引入第 3 周才学习的完整 MVVM、`ICommand` 或第三方 MVVM 框架。

## 2. 范围

### 2.1 包含

- WPF、XAML、`Grid`、`StackPanel`、`Border`。
- `DataContext`、`Binding` 和 `IValueConverter`。
- `ItemsControl`、`DataGrid`、Resource、Style、DataTemplate。
- 后台假数据采集、`Dispatcher` UI 线程切换和窗口关闭生命周期。
- 启动、停止、复位操作。
- 温度、压力、转速、累计产量及最近 20 条采样。
- 状态文字和颜色随数值等级变化。
- Core、App、Tests 三项目分层及 xUnit 自动化测试。
- 1280×720、1920×1080 两种分辨率的人工验收和截图。
- 验证通过后创建本地 `V1` Tag。

### 2.2 不包含

- 完整 MVVM、ViewModel、RelayCommand、依赖注入容器。
- Modbus、S7、OPC UA 或真实设备通信。
- 数据库、报警历史、趋势图、权限、安装包。
- 自动推送 Git 提交或 Tag 到远端。

## 3. 方案选择

采用独立的 `App + Core + Tests` 三项目方案：

```text
02/
├─ ProductionLineMonitor.slnx
├─ ProductionLineMonitor.Core/
│  ├─ Models/
│  │  ├─ DeviceReading.cs
│  │  ├─ DeviceStatus.cs
│  │  └─ MetricLevel.cs
│  └─ Services/
│     ├─ FakeDataGenerator.cs
│     ├─ AcquisitionService.cs
│     ├─ ReadingEvaluator.cs
│     └─ RecentReadingBuffer.cs
├─ ProductionLineMonitor.App/
│  ├─ Converters/
│  ├─ Models/
│  ├─ Themes/
│  ├─ App.xaml
│  ├─ MainWindow.xaml
│  └─ MainWindow.xaml.cs
├─ ProductionLineMonitor.Tests/
├─ docs/
│  └─ screenshots/
└─ README.md
```

`Core` 不引用 WPF。`App` 只负责展示、用户操作和 UI 线程调度。`Tests` 只测试纯 C# 核心行为和异步生命周期。

没有选择直接引用第 1 周控制台项目，因为那会让第 2 周依赖一个 `Exe` 项目并形成跨周耦合。没有选择单一 WPF 项目，因为它会削弱业务逻辑与界面之间的测试边界。

## 4. 核心模型与规则

### 4.1 采样模型

`DeviceReading` 是一次不可变采样，至少包含：

- 时间戳。
- 温度，单位 °C。
- 压力，单位 MPa。
- 转速，单位 rpm。
- 本次采样后的累计产量。
- 温度、压力、转速各自的状态等级。
- 三项指标中的最高等级，作为综合状态。

### 4.2 状态等级

`MetricLevel` 包含 `Normal`、`Warning`、`Alarm`。业务层只返回枚举，不返回 WPF 的 `Brush`。

阈值如下，边界按表中闭区间处理：

| 指标 | 正常 | 警告 | 报警 |
|---|---|---|---|
| 温度 | 15–35 | 10–15 及 35–40 中排除正常边界后的部分 | 小于 10 或大于 40 |
| 压力 | 0.1–0.5 | 0.05–0.1 及 0.5–0.6 中排除正常边界后的部分 | 小于 0.05 或大于 0.6 |
| 转速 | 1000–2000 | 500–1000 及 2000–2500 中排除正常边界后的部分 | 小于 500 或大于 2500 |

为消除边界歧义，判断顺序固定为：先判断正常闭区间，再判断警告闭区间，最后为报警。因此 15、35、0.1、0.5、1000 和 2000 属于正常；10、40、0.05、0.6、500 和 2500 属于警告。

### 4.3 假数据

假数据覆盖正常、警告和报警范围，以便观察状态颜色变化。每次采样累计产量增加一个小的正整数。`Reset()` 将累计产量归零。

`FakeDataGenerator` 不负责循环或线程，只负责生成一条数据。这样随机生成和采集生命周期可以分别测试。

### 4.4 最近 20 条

`RecentReadingBuffer` 按“最新数据在最前”保存样本。加入第 21 条时移除最旧一条，任意时刻数量都不超过 20。复位会清空缓冲区。

## 5. 采集与线程设计

`AcquisitionService` 在后台任务中运行采集循环：

1. 检查取消令牌。
2. 生成一条假数据。
3. 调用可等待的数据回调。
4. 使用带取消令牌的 `Task.Delay` 等待一个采样周期。

生产环境采样周期为 1 秒；测试可以注入更短周期。服务保存正在运行的任务，重复启动不会创建第二个循环。

后台服务不引用 `Dispatcher`、`Window`、控件或界面集合。窗口接收采样后只执行一次 `Dispatcher.InvokeAsync`，在 UI 线程中：

- 调用 `Dispatcher.VerifyAccess()`。
- 更新最近 20 条缓冲。
- 创建新的不可变 `DashboardState`。
- 将窗口的 `DataContext` 替换为新状态。

V1 有意使用“替换绑定快照”的方式，不实现 `INotifyPropertyChanged` 和 `ObservableCollection`；这些内容留给第 3 周的 MVVM 重构。1 Hz、20 条数据的规模下，重新绑定不会造成可感知卡顿。

## 6. 操作与生命周期

### 6.1 状态矩阵

| 运行状态 | 启动 | 停止 | 复位 |
|---|---:|---:|---:|
| 已停止 | 可用 | 禁用 | 可用 |
| 运行中 | 禁用 | 可用 | 禁用 |
| 正在停止/关闭 | 禁用 | 禁用 | 禁用 |

模拟设备在程序打开期间保持在线；在线状态和运行状态分别显示。

### 6.2 启动

启动时创建新的 `CancellationTokenSource`，保存采集任务，并更新绑定状态。已运行时再次启动不执行任何操作。

### 6.3 停止

停止时先发出取消信号，再 `await` 采集任务结束，最后释放 `CancellationTokenSource` 并恢复已停止状态。与当前令牌匹配的 `OperationCanceledException` 是正常控制流，不显示为错误。

### 6.4 复位

复位仅在已停止时可用。复位会清空当前读数、最近 20 条记录和累计产量，但不会改变设备名称及在线状态。

### 6.5 关闭窗口

第一次触发 `Closing` 时暂时取消关闭，禁用操作，调用与停止相同的异步清理逻辑并等待后台任务退出。清理完成后设置关闭完成标志，再次调用 `Close()`；第二次 `Closing` 允许真正关闭。

`async void` 仅存在于 WPF 事件入口，所有业务方法返回 `Task`。代码不使用 `.Wait()`、`.Result` 或强制终止线程。

## 7. 界面设计

窗口采用同一套自适应 XAML：

```text
主窗口
├─ 顶栏：设备名称、在线状态、运行状态、启动/停止/复位
├─ 指标区：ItemsControl + 四列 UniformGrid
│  ├─ 温度
│  ├─ 压力
│  ├─ 转速
│  └─ 累计产量
├─ 采样区：DataGrid
│  └─ 时间、温度、压力、转速、产量、综合状态
└─ 状态栏：采样周期、最后更新时间、当前提示
```

布局使用 `Grid` 的 `Auto` 与 `*` 行高，主内容最大宽度为 1600 DIP 并居中。1280×720 下四张卡保持并排，DataGrid 使用内部滚动条访问 20 条数据；1920×1080 下增加两侧留白和表格可视行数。

不使用全窗口 `Viewbox`，不在 DataGrid 外嵌套额外 `ScrollViewer`。窗口设置安全的最小宽高、`UseLayoutRounding` 和 `SnapsToDevicePixels`。DataGrid 开启行列虚拟化。

`Colors.xaml` 保存颜色资源，`Controls.xaml` 保存按钮、状态徽标、指标卡、DataGrid 样式和指标卡 DataTemplate。`MetricLevelToBrushConverter` 将状态枚举映射为主题颜色。所有状态同时显示文字和颜色，不只依赖颜色传达信息。

## 8. 错误处理

- 正常取消不显示错误。
- 非取消异常会被观察，并通过 Dispatcher 在状态栏显示简短错误信息。
- 异常后释放本次采集资源并回到已停止状态。
- 启动、停止和关闭逻辑保持幂等。
- 窗口关闭后不再接受采样，不产生未观察任务异常。

## 9. 测试设计

使用仓库第 1 周已经采用的 xUnit 版本，不引入 MVVM 或 UI 自动化依赖。遵循 RED → GREEN → REFACTOR：先运行每个新测试并确认因功能缺失而失败，再实现最小代码。

自动化测试覆盖：

- 每个数值阈值的下方、等于和上方。
- `NaN` 与无穷值判为报警。
- 最近 0、1、19、20、21、25 条的容量和顺序。
- 假数据落在允许范围内，产量递增，复位后归零。
- 短采样周期下每个周期发布数据。
- 取消后任务及时结束，样本数量不再增加。
- 重复启动不创建第二个采集任务。
- 重复停止不抛异常。

WPF 视觉、Dispatcher 和窗口生命周期由人工验收补充，自动化测试不会被描述成完整 UI 验收。

## 10. 验收与交付

自动检查：

```powershell
dotnet test .\02\ProductionLineMonitor.slnx -c Release
dotnet build .\02\ProductionLineMonitor.slnx -c Release --no-restore -warnaserror
git diff --check -- 02
```

人工检查：

- 运行至少 30 秒，确认每秒一条数据且操作、拖动和调整窗口不卡顿。
- 运行中关闭窗口，确认后台任务结束且进程退出。
- 在 Windows 1280×720 和 1920×1080 下分别最大化验收，并记录显示缩放比例。
- 内容不重叠，按钮可操作，20 条记录均可通过 DataGrid 滚动访问。

截图保存为：

- `02/docs/screenshots/V1-1280x720.png`
- `02/docs/screenshots/V1-1920x1080.png`

截图加入 README 后，只显式暂存和提交 `02` 下文件。验证目标提交正确后创建 annotated `V1` Tag。推送远端不在本任务默认范围内。

## 11. 新手友好约定

- 注释解释“为什么”和线程/生命周期边界，不逐行重复语法。
- 对关键 C# 概念补充简短 Java 对照，但避免在每个文件重复大段教材。
- 每个类保持单一职责，名称直接表达用途。
- README 给出项目结构、运行命令、操作步骤、线程安全说明和 V2 重构方向。
- 不隐藏关键逻辑于第三方框架或代码生成器。
