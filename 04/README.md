## Modbus TCP 设备通信

### 目标

实现 Modbus TCP 通信抽象层，通过接口隔离真实设备和模拟设备，便于开发和测试。

### 核心设计

```
04/
├── ModbusMonitor.slnx
├── ModbusMonitor.Core/
│   ├── Models/
│   │   └── DeviceData.cs           # 从设备读取的数据点
│   ├── Communication/
│   │   ├── IModbusClient.cs        # 通信接口（抽象）
│   │   └── SimulatedModbusClient.cs # 模拟实现（开发测试用）
│   └── Services/
│       └── DeviceReader.cs         # 寄存器 → 业务数据转换
├── ModbusMonitor.Simulator/        # 控制台模拟器演示
└── ModbusMonitor.Tests/
```

### Modbus 寄存器映射

| 寄存器 | 地址 | 数据 | 转换公式 |
|--------|------|------|----------|
| 温度 | 0 | ushort | 值 / 100 = °C |
| 压力 | 1 | ushort | 值 / 1000 = MPa |
| 转速 | 2 | ushort | 值 = rpm |
| 产量 | 3 | ushort | 值 = 件 |

### Java vs C# 对照

| 概念 | Java | C# |
|------|------|-----|
| 接口 | `interface` | `interface` (完全相同) |
| 异步 | `CompletableFuture` | `Task<T>` + `async/await` |
| 资源释放 | `AutoCloseable` | `IAsyncDisposable` |
| 依赖注入 | `@Autowired` | 构造函数注入 |

### 运行命令

```bash
# 构建
dotnet build ./04/ModbusMonitor.slnx -c Release

# 运行测试
dotnet test ./04/ModbusMonitor.slnx -c Release

# 启动模拟器
dotnet run --project ./04/ModbusMonitor.Simulator/ModbusMonitor.Simulator.csproj
```

### V2 扩展方向

- 接入真实 Modbus TCP 库（NModbus4 / FluentModbus）
- 添加连接重试和断线重连
- 支持写入寄存器（控制设备）
- 多设备并发读取
