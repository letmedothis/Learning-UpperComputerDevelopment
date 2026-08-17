## Web API + 远程监控

### 目标

使用 ASP.NET Core Web API 实现远程设备监控接口，支持设备状态查询和历史数据获取。

### 项目结构

```
08/
├── RemoteMonitoring.slnx
├── RemoteMonitoring.Core/
│   ├── Models/
│   │   ├── MachineState.cs      # 设备状态模型
│   │   └── DeviceSummary.cs     # 设备摘要
│   └── Services/
│       ├── IDeviceStateService.cs      # 服务接口
│       └── SimulatedDeviceStateService.cs # 模拟实现
├── RemoteMonitoring.Api/
│   ├── Controllers/
│   │   ├── DevicesController.cs # 设备 API
│   │   └── HealthController.cs  # 健康检查
│   └── Program.cs
└── RemoteMonitoring.Tests/
```

### API 接口

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/devices` | 获取所有设备列表 |
| GET | `/api/devices/{id}/state` | 获取设备当前状态 |
| GET | `/api/devices/{id}/history?count=50` | 获取设备历史状态 |
| GET | `/api/health` | 健康检查 |

### 运行命令

```bash
# 构建
dotnet build ./08/RemoteMonitoring.slnx -c Release

# 运行测试
dotnet test ./08/RemoteMonitoring.slnx -c Release

# 启动 API（默认端口 5000）
dotnet run --project ./08/RemoteMonitoring.Api/RemoteMonitoring.Api.csproj

# 访问 Swagger UI
# http://localhost:5000/swagger
```

### 测试 API

```bash
# 获取所有设备
curl http://localhost:5000/api/devices

# 获取设备状态
curl http://localhost:5000/api/devices/CNC-001/state

# 获取历史数据
curl http://localhost:5000/api/devices/CNC-001/history?count=10

# 健康检查
curl http://localhost:5000/api/health
```
