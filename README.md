# Java 开发转上位机开发学习项目

这是一个面向 Java 开发者的 C# / WPF / 工业上位机学习仓库。

项目以“产线设备监控”为练习场景，通过多个可独立运行的章节，逐步学习 C#、WPF、MVVM、设备通信抽象、数据持久化、趋势图、报警、Web API、测试与发布。

> 本仓库的首要目标是学习和验证知识，不是直接交付生产产品。当前大部分设备数据来自模拟器；没有真实 PLC 验证的能力会明确标注，不把模拟结果描述为现场联调结果。

## 学习目标

这条路线不要求重新学习变量、循环和面向对象，而是把已有 Java 工程经验迁移到 .NET，并补齐工业通信与现场边界：

- 使用 C#、WPF 和 MVVM 编写可维护的 Windows 桌面程序。
- 理解异步、取消、资源释放、UI 线程切换和并发保护。
- 能阅读点表，处理地址、数据类型、倍率、单位和读写权限。
- 掌握设备轮询、超时、异常、重连和数据质量的基本设计。
- 完成趋势、报警、SQLite 历史数据和远程查询等典型功能练习。
- 最终理解 Modbus TCP、Siemens S7 和 OPC UA 的使用场景与差异。
- 让每一阶段都具备代码、测试、运行方法、已知限制和可解释的设计取舍。

长期能力模型：

```text
软件工程能力 + 工业通信能力 + 现场排障能力 + 工艺理解能力
```

## 总体学习主线

```text
Java 开发经验
    ↓
C# 差异化学习
    ↓
WPF 基础与 UI 线程
    ↓
MVVM 与可测试设计
    ↓
Modbus 风格设备通信与点表
    ↓
稳定采集、配置、日志与重连
    ↓
趋势图、报警和历史数据库
    ↓
Siemens S7 / OPC UA
    ↓
部署、排障和作品集
```

当前仓库主要覆盖前半段和部分应用功能。稳定采集闭环、真实 Modbus TCP、S7 和 OPC UA 仍属于后续任务。

## Java 与 C# / WPF 迁移重点

| Java / Java Web | C# / .NET / WPF | 本项目关注点 |
|---|---|---|
| JDK / JVM | .NET SDK / CLR | 目标框架、程序集、构建与运行 |
| Maven / Gradle | NuGet + MSBuild | `.csproj`、依赖和发布参数 |
| getter / setter | Property | 自动属性、`init`、属性通知 |
| Stream API | LINQ | 过滤、投影、分组和聚合 |
| CompletableFuture | `Task` + `async/await` | 异常传播、取消和线程切换 |
| `synchronized` / Lock | `lock` / `SemaphoreSlim` | 单连接串行化和共享状态保护 |
| Listener | delegate / event | 数据发布、订阅和生命周期 |
| try-with-resources | `using` / `await using` | `IDisposable` 与连接释放 |
| Spring DI | .NET 依赖注入 | 构造函数注入和服务生命周期 |
| JPA / MyBatis | EF Core / ADO.NET / Dapper | ORM、SQL、迁移和持久化边界 |
| Swing / JavaFX | WPF | XAML、Binding、Command、Dispatcher |
| Observer | `INotifyPropertyChanged` | ViewModel 驱动 UI 更新 |

优先掌握：Property、nullable、`record`、委托与事件、LINQ、`Task`、`CancellationToken`、`IDisposable`、依赖注入、配置、日志和单元测试。

## 当前章节

| 章节 | 主题 | 当前实现 | 仍需注意 |
|---|---|---|---|
| [01](./01/readme.md) | C# 差异化学习 | C# 语法对照、数据生成器、LINQ 分析、异步取消、资源生命周期和测试 | 依赖注入以构造函数示例为主，尚未形成完整宿主配置 |
| [02](./02/README.md) | WPF 设备监控 V1 | XAML、Binding、样式、DataGrid、Dispatcher、启动/停止和有界读数 | 使用模拟采集；V1 刻意保留较多 code-behind |
| [03](./03/README.md) | MVVM 重构 | 属性通知、Command、ObservableCollection、状态与业务分离 | 仍需进一步引入可替换设备客户端和真正的异步命令 |
| [04](./04/README.md) | Modbus 风格通信练习 | `IModbusClient` 抽象、寄存器读取、倍率转换、短响应校验 | 当前是模拟寄存器客户端，不是实际 Modbus TCP 报文通信 |
| [05](./05/README.md) | SQLite + EF Core | DbContext、仓储、时间范围查询、关闭并重开后的持久化测试 | 尚未演示正式迁移、批量写入和长期数据保留策略 |
| [06](./06/README.md) | 历史趋势图 | 历史序列、统计、Canvas / StreamGeometry 绘图 | 当前是静态历史绘图，不是实时滚动曲线 |
| [07-new](./07-new/README.md) | 报警规则引擎 | 阈值比较、等级、消息模板、冷却和稳定规则标识 | 尚未实现确认、恢复、滞回、延时等完整报警生命周期 |
| [08](./08/README.md) | Web API + 远程监控 | Controller、DI、设备状态查询、有界并发历史 | 数据仍为模拟；当前重点是 API 与并发边界，不是生产远程控制 |
| [09](./09/README.md) | 测试运行器与发布练习 | 测试步骤聚合、发布命令和 Dockerfile 内容生成 | 是部署知识练习，不是完整 CI/CD 或自动部署平台 |

章节是逐步学习用的小项目，目前没有强行合并成一个“大而全”解决方案。后续综合项目应复用这些已经验证的概念，而不是简单复制代码。

## 工业开发必须守住的边界

### 上位机与 PLC 的职责

```text
WPF 上位机
  监控 / 参数 / 配方 / 报警 / 历史记录
        ↓
通信驱动（Modbus / S7 / OPC UA）
        ↓
PLC
  确定性控制 / 联锁 / 关键实时逻辑
        ↓
传感器 / 电机 / 变频器 / 气缸 / 仪表
```

- 安全联锁和关键实时控制不能只放在上位机中。
- “通信写入成功”不等于“机械动作完成”，必须读取反馈信号确认。
- 真实设备写入前，应确认地址、权限、急停、联锁和现场人员安全。
- 模拟验证、实验室验证与真实 PLC 联调必须明确区分。

### 点表是核心接口

一份可用点表至少应包含：

| 字段 | 示例 | 需要回答的问题 |
|---|---|---|
| 点位名称 | 温度 | 业务含义是否明确？ |
| 协议地址 | Holding Register 0 | 文档从 0 还是从 1 计数？ |
| 数据类型 | `Int16` / `UInt32` / `Float` | 跨几个寄存器？是否有符号？ |
| 读写属性 | R / W | 是否允许写入？是否需要握手？ |
| 倍率与单位 | 0.1 ℃ | 原始值如何转换？ |
| 字节序 / 字序 | ABCD / CDAB | 32 位值如何组合？ |
| 采集周期 | 500 ms | 是否适合批量读取？ |
| 上下限 | 0～100 ℃ | 越界时如何处理？ |

尤其要理解：设备文档中的 `40001` 常是展示编号，而客户端库可能要求传入偏移量 `0`。

## 学习和实现原则

- 优先使用模拟器建立闭环，后续再接入真实 PLC。
- 先验证协议、时序、异常和恢复，再打磨 UI。
- 不在 UI 线程中执行阻塞式通信或数据库写入。
- 设备访问需要考虑超时、取消、错误记录和重连。
- 点位、倍率、周期、读写权限和报警阈值应逐步配置化。
- 采集频率与 UI 刷新频率应解耦，历史集合必须有容量上限。
- 每个关键行为至少覆盖一个正常路径和一个失败路径测试。
- 测试通过只证明被测试的行为，不等于完成真实设备或完整集成验证。
- 不追求一次性做成产品；每次只增加一个可运行、可解释、可验收的学习切片。

## 环境要求

- Windows 10 / 11
- .NET SDK 10
- Visual Studio（安装“.NET 桌面开发”工作负载）或其他支持 .NET 的 IDE
- Git
- SQLite 查看工具可选
- 后续真实通信阶段再准备 Modbus TCP 模拟服务器、PLCSIM / Siemens PLC 或 OPC UA 示例服务器

WPF 是 Windows 桌面技术，本路线以 Windows 工控机场景为背景，不追求跨平台 UI。

## 快速开始

先确认 SDK：

```powershell
dotnet --info
```

运行某一章测试，例如：

```powershell
dotnet test .\03\MvvmMonitor.slnx -c Release
dotnet test .\05\DataPersistence.slnx -c Release
dotnet test .\08\RemoteMonitoring.slnx -c Release
```

启动 WPF 示例：

```powershell
dotnet run --project .\02\ProductionLineMonitor.App\ProductionLineMonitor.App.csproj
dotnet run --project .\03\MvvmMonitor.App\MvvmMonitor.App.csproj
dotnet run --project .\06\TrendChart.App\TrendChart.App.csproj
```

启动其他示例：

```powershell
# Modbus 风格模拟器
dotnet run --project .\04\ModbusMonitor.Simulator\ModbusMonitor.Simulator.csproj

# 远程监控 API
dotnet run --project .\08\RemoteMonitoring.Api\RemoteMonitoring.Api.csproj
```

更具体的结构、命令和知识点见各章节 README。

## 每阶段验收方式

每个学习切片完成时，应至少留下这些证据：

- 功能可以独立启动或由自动化测试执行。
- 至少一个正常路径测试和一个失败路径测试。
- 对取消、异常、资源释放或输入边界有明确处理。
- README 写明运行方式、已完成内容和已知限制。
- 能解释一次故障的现象、原因、修复和防复发方式。
- 模拟验证不冒充真实设备验证。

推荐复盘模板：

```markdown
# 阶段复盘

## 本阶段交付
-

## 我真正理解的概念
-

## 故障与证据
- 现象：
- 原因：
- 修复：
- 如何防止复发：

## 下一阶段风险
-
```

## 后续优先顺序

1. 抽象可替换的 `IDeviceClient`，把 03 的 ViewModel 与具体模拟器解耦。
2. 把 04 从“类 Modbus 寄存器练习”升级为本地真实 Modbus TCP Client / Server 闭环。
3. 增加稳定采集层：点表配置、批量读取、超时、重连、状态机和数据质量。
4. 串联设备读取 → 报警规则 → SQLite → 历史曲线 → Web API。
5. 为报警补齐发生、确认、恢复、滞回、延时和历史查询。
6. 在模拟链路稳定后学习 Siemens S7，并明确是否完成真实 PLC 验证。
7. 最后学习 OPC UA 的 Session、NodeId、Subscription、StatusCode 和证书信任。
8. 补充架构图、点表样例、故障注入记录、部署说明和演示材料。

## 暂未实现

以下内容属于路线目标，不代表当前仓库已经完成：

- 真实 Modbus TCP 报文通信、功能码和异常码处理
- 点表驱动的稳定轮询、断线重连和数据质量模型
- Siemens S7 DB / M / I / Q 读写
- OPC UA 浏览、读写、订阅和证书管理
- 完整报警生命周期和后台批量入库
- 真实 PLC 写入及现场安全联调
- 长时间稳定性、性能和工控机部署验证

学习过程中应持续更新这一节，避免文档能力声明领先于代码和测试证据。
