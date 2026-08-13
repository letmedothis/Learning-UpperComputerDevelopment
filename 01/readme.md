## C# 差异化学习

### 目标

把已有 Java 能力迁移到 C#，能独立编写异步、可取消、可测试的基础逻辑。

### 学习路径

按照以下顺序学习，每个知识点都有对应的示例代码：

```
C#Differences/
└── Demos/
    ├── 01_Property/              # 属性基础
    ├── 02_NullableReference/     # 可空引用类型
    ├── 03_Records/               # record 类型
    ├── 04_DelegatesEvents/       # 委托、事件、Lambda
    ├── 05_LINQ/                  # LINQ 查询
    ├── 06_AsyncAwait/            # Task 和 async/await + CancellationToken
    ├── 07_Disposable/            # IDisposable 资源释放
    └── 08_ConfigLoggingDI/       # 配置、日志、依赖注入
```

### 知识点总结

#### 1. Property（属性）

**Java 对比**：Java 使用 getter/setter 方法，C# 用属性语法糖简化。

```csharp
// C# 自动属性 - 编译器自动生成私有字段
public string Name { get; set; } = string.Empty;

// 只读属性 - 只能在构造函数中赋值
public decimal Price { get; }

// 带验证的属性 - set 中可加逻辑
private int _age;
public int Age
{
    get => _age;
    set => _age = value >= 0 ? value : throw new ArgumentException("年龄不能为负");
}

// 计算属性 - 不存储值，每次调用时计算
public double Area => Width * Height;
```

#### 2. 可空引用类型

**Java 对比**：Java 用 `@Nullable` 注解（仅提示），C# 在编译期强制检查。

```csharp
#nullable enable  // 启用可空检查

string name = "张三";    // 非空，不能赋 null
string? nickname = null; // 可空，可以赋 null

// 安全访问
int? length = nickname?.Length;      // 空值条件运算符
string display = nickname ?? "匿名"; // 空值合并运算符
```

#### 3. record 类型

**Java 对比**：类似 Lombok `@Value` 或 Java 16+ `record`，但功能更强。

```csharp
// record - 不可变数据类型，自带值相等性
public record Point(int X, int Y);

var p1 = new Point(3, 4);
var p2 = new Point(3, 4);
p1 == p2;           // true（值相等）
p1 with { X = 10 }; // 创建副本并修改
var (x, y) = p1;    // 解构
```

#### 4. 委托、事件、Lambda

**Java 对比**：Java 用函数式接口 + lambda，C# 有专门的 `delegate` 和 `event` 关键字。

```csharp
// 委托 - 类型安全的函数引用
delegate int MathOperation(int a, int b);
MathOperation add = (a, b) => a + b;

// 事件 - 发布/订阅模式
public event EventHandler<DataEventArgs>? DataReceived;
DataReceived?.Invoke(this, new DataEventArgs("温度", 25.5));

// 内置委托类型
Action<string> print = Console.WriteLine;  // 无返回值
Func<int, int, int> add = (a, b) => a + b; // 有返回值
Predicate<int> isEven = n => n % 2 == 0;   // 返回 bool
```

#### 5. LINQ

**Java 对比**：类似 Stream API，但语法更灵活（支持查询语法和方法语法）。

```csharp
// 查询语法（类似 SQL）
var result = from e in employees
             where e.Department == "技术部"
             orderby e.Salary descending
             select new { e.Name, e.Salary };

// 方法语法（链式调用）
var result = employees
    .Where(e => e.Department == "技术部")
    .OrderByDescending(e => e.Salary)
    .Select(e => new { e.Name, e.Salary });

// 常用操作
employees.GroupBy(e => e.Department);  // 分组
employees.Average(e => e.Salary);      // 聚合
employees.Join(departments, ...);      // 连接
employees.Take(10).Skip(20);           // 分页
```

#### 6. Task 和 async/await

**Java 对比**：Java 用 `CompletableFuture` + 链式调用，C# 用 `async/await` 让异步代码看起来像同步。

```csharp
// Task - 表示异步操作（类似 CompletableFuture）
// async/await - 语法糖，编译器自动生成状态机

async Task<double> ReadSensorAsync()
{
    await Task.Delay(100); // 异步等待，不阻塞线程
    return 25.5;
}

// 并行执行
Task<double> tempTask = ReadSensorAsync("温度");
Task<double> pressureTask = ReadSensorAsync("压力");
double[] results = await Task.WhenAll(tempTask, pressureTask);

// 关键理解：
// - Task ≠ 线程！Task 是"承诺"，可能在线程池中执行
// - await 不阻塞线程，而是将后续代码注册为回调
```

#### 7. CancellationToken

**Java 对比**：Java 无内置取消机制（用 volatile boolean），C# 提供协作式取消。

```csharp
// CancellationTokenSource - 取消的控制者
using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(5)); // 5秒后自动取消

// CancellationToken - 传递给异步方法
async Task CollectDataAsync(CancellationToken cancellationToken)
{
    while (!cancellationToken.IsCancellationRequested)
    {
        // 检查取消状态
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay(1000, cancellationToken); // Delay 也支持取消
    }
}

// 链式取消 - 多个取消条件组合
using var linked = CancellationTokenSource.CreateLinkedTokenSource(
    userToken, timeoutToken);
```

#### 8. IDisposable

**Java 对比**：Java 用 `AutoCloseable` + try-with-resources，C# 用 `IDisposable` + `using`。

```csharp
// 实现 IDisposable - 释放非托管资源
public class DatabaseConnection : IDisposable
{
    public void Dispose()
    {
        // 释放资源
    }
}

// using 语句 - 确保 Dispose 被调用
using (var conn = new DatabaseConnection())
{
    conn.ExecuteQuery("...");
} // 自动调用 Dispose()

// using 声明 (C# 8.0+) - 更简洁
using var conn = new DatabaseConnection();
// 方法结束时自动调用 Dispose()
```

#### 9. 配置、日志、依赖注入

**Java 对比**：Spring Boot 内置，C# 通过 `Microsoft.Extensions.*` 提供。

```csharp
// 配置 - IConfiguration
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
string connStr = config.GetConnectionString("Default");

// 日志 - ILogger
ILogger<MyClass> logger = loggerFactory.CreateLogger<MyClass>();
logger.LogInformation("传感器 {Name} 读数: {Value}", name, value);

// 依赖注入 - IServiceCollection
services.AddSingleton<ISensorService, SensorService>();
var service = serviceProvider.GetRequiredService<ISensorService>();
```

### 动手任务

#### 设备数据生成器

综合应用所有知识点的完整项目：

```
DeviceDataGenerator/
├── DeviceDataGenerator/
│   ├── Program.cs                  # 主程序
│   ├── Models/
│   │   └── DeviceReading.cs        # record 数据模型 + Property 配置
│   ├── Services/
│   │   ├── DeviceDataGenerator.cs  # 事件 + async/await + CancellationToken
│   │   └── DataAnalyzer.cs         # LINQ 数据分析
│   └── Utils/
│       └── SensorUtils.cs          # 倍率转换 + 范围判断
└── DeviceDataGenerator.Tests/
    └── SensorUtilsTests.cs         # 57 个单元测试 ✅
```

**功能要求**：
- ✅ 每秒生成温度、压力、转速
- ✅ 用事件发布新数据
- ✅ 使用 `CancellationToken` 停止采集
- ✅ 用 LINQ 计算最近 60 个温度值的最大、最小、平均值
- ✅ 为倍率转换、范围判断写单元测试

### 验收标准

- [x] 程序可连续运行 10 分钟
- [x] 可正常取消，没有强制终止线程
- [x] 异步方法不使用无意义的 `.Result` 或 `.Wait()`
- [x] 能向别人解释 `Task` 与线程不是同一个概念
- [x] 至少有 5 个通过的单元测试（实际 57 个）

### Java vs C# 速查表

| 概念 | Java | C# |
|------|------|-----|
| 属性 | getter/setter | `{ get; set; }` |
| 不可变数据 | `record` / Lombok `@Value` | `record` |
| 空安全 | `@Nullable` 注解 | `string?` + 编译器检查 |
| 函数引用 | 函数式接口 | `delegate` / `Action` / `Func` |
| 事件 | 自定义监听器接口 | `event` 关键字 |
| 流式操作 | Stream API | LINQ |
| 异步 | CompletableFuture | Task + async/await |
| 取消 | volatile boolean | CancellationToken |
| 资源释放 | AutoCloseable + try-with-resources | IDisposable + using |
| 依赖注入 | Spring IoC | Microsoft.Extensions.DI |
| 日志 | Log4j / SLF4J | ILogger |
| 配置 | application.yml | appsettings.json + IConfiguration |
