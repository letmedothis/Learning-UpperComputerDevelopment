## 数据持久化（SQLite + EF Core）

### 目标

使用 Entity Framework Core + SQLite 实现机床状态数据的持久化存储和查询。

### 项目结构

```
05/
├── DataPersistence.slnx
├── DataPersistence.Core/
│   ├── Entities/
│   │   └── MachineStateRecord.cs     # 数据库实体
│   └── Repositories/
│       └── IMachineStateRepository.cs # 仓储接口
├── DataPersistence.Data/
│   ├── MonitoringDbContext.cs         # EF Core 上下文
│   └── Repositories/
│       └── MachineStateRepository.cs  # 仓储实现
└── DataPersistence.Tests/
    └── Repositories/
        └── MachineStateRepositoryTests.cs
```

### 数据表结构

```sql
CREATE TABLE MachineStates (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp       TEXT NOT NULL,
    ConnectionStatus INTEGER,
    RunStatus        INTEGER,
    WorkpieceCount   INTEGER,
    CurrentProgramName TEXT,
    SpindleSpeed     REAL,
    SpindleLoad      REAL,
    FeedSpeed        REAL,
    CurrentAlarm     TEXT
);
CREATE INDEX IX_MachineStates_Timestamp ON MachineStates(Timestamp);
```

### Java vs C# 对照

| 概念 | Java | C# |
|------|------|-----|
| ORM | Hibernate / JPA | Entity Framework Core |
| 依赖注入 | `@Autowired` | 构造函数注入 |
| 内存数据库 | H2 | SQLite `:memory:` |
| 异步查询 | `CompletableFuture` | `async/await` |

### 运行命令

```bash
dotnet build ./05/DataPersistence.slnx -c Release
dotnet test ./05/DataPersistence.slnx -c Release
```
