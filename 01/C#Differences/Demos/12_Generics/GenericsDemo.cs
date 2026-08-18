namespace C_Differences.Demos._12_Generics;

/// <summary>
/// 泛型差异演示 - Java 类型擦除 vs C# 具化泛型
///
/// 【为什么泛型差异很重要】
/// 泛型是现代编程的核心特性。Java 和 C# 的泛型实现有根本性差异，
/// 理解这些差异能帮你写出更高效、更安全的代码。
///
/// 【Java 对比总览】
/// ┌─────────────────┬─────────────────────────┬─────────────────────────┐
/// │ 特性             │ Java 泛型               │ C# 泛型                 │
/// ├─────────────────┼─────────────────────────┼─────────────────────────┤
/// │ 实现方式         │ 类型擦除 (Type Erasure) │ 具化 (Reified)          │
/// │ 运行时类型信息   │ ❌ 丢失                 │ ✅ 保留                 │
/// │ 创建实例         │ ❌ 不能 new T()         │ ✅ 可以 new T()         │
/// │ 类型检查         │ ❌ 不能 instanceof T    │ ✅ 可以 is T            │
/// │ 泛型数组         │ ❌ 不能 new T[]         │ ✅ 可以 new T[]         │
/// │ 性能             │ 需要装箱/拆箱           │ 无需装箱/拆箱           │
/// │ 约束语法         │ <T extends Comparable>  │ where T : IComparable   │
/// │ 协变/逆变        │ ? extends / ? super     │ out / in                │
/// └─────────────────┴─────────────────────────┴─────────────────────────┘
///
/// 【Java 泛型的类型擦除】
/// <code>
/// // Java 泛型在编译后会被擦除
/// List<String> strings = new ArrayList<>();
/// List<Integer> integers = new ArrayList<>();
///
/// // 运行时无法区分
/// strings.getClass() == integers.getClass() // true! 都是 ArrayList
///
/// // 无法创建泛型数组
/// // new List<String>[10] // 编译错误!
///
/// // 无法检查泛型类型
/// // if (list instanceof List<String>) // 编译错误!
/// </code>
///
/// 【C# 泛型的具化特性】
/// <code>
/// // C# 泛型在运行时保留类型信息
/// List<string> strings = new List<string>();
/// List<int> integers = new List<int>();
///
/// // 运行时可以区分
/// strings.GetType() != integers.GetType() // true!
///
/// // 可以创建泛型数组
/// var array = new List<string>[10]; // 合法
///
/// // 可以检查泛型类型
/// if (strings is List<string>) { ... } // 合法
/// </code>
///
/// 【性能影响】
/// Java 泛型：需要装箱/拆箱，有性能损耗
/// C# 泛型：无需装箱/拆箱，性能更好
///
/// 【学习建议】
/// 1. 理解类型擦除 vs 具化的根本差异
/// 2. 掌握泛型约束的语法差异
/// 3. 了解协变和逆变的使用场景
/// 4. 在实际项目中善用泛型提高代码复用性
/// </summary>
public class GenericsDemo
{
    public void Demo()
    {
        Console.WriteLine("=== 泛型差异学习 ===\n");

        Console.WriteLine("1. 类型擦除 vs 具化泛型:");
        TypeErasureVsReified();

        Console.WriteLine("\n2. 泛型约束:");
        GenericConstraints();

        Console.WriteLine("\n3. 协变和逆变:");
        CovarianceAndContravariance();

        Console.WriteLine("\n4. 泛型方法:");
        GenericMethods();

        Console.WriteLine("\n5. 泛型类:");
        GenericClasses();

        Console.WriteLine("\n6. 实际应用 - 通用仓储模式:");
        PracticalExample();
    }

    /// <summary>
    /// 类型擦除 vs 具化泛型
    ///
    /// 【关键区别】
    /// Java: 运行时无法获取泛型类型信息
    /// C#:   运行时保留完整的类型信息
    /// </summary>
    private void TypeErasureVsReified()
    {
        // Java 的类型擦除示例（伪代码）：
        // List<String> strings = new ArrayList<>();
        // List<Integer> integers = new ArrayList<>();
        // strings.getClass() == integers.getClass() // true! 都是 ArrayList

        // C# 的具化泛型
        var strings = new List<string>();
        var integers = new List<int>();

        // 运行时可以获取类型信息
        Console.WriteLine($"   strings 类型: {strings.GetType().Name}"); // List`1
        Console.WriteLine($"   integers 类型: {integers.GetType().Name}"); // List`1

        // 获取泛型参数
        Type stringListType = typeof(List<string>);
        Type intListType = typeof(List<int>);

        Console.WriteLine($"   List<string> 泛型参数: {stringListType.GetGenericArguments()[0].Name}");
        Console.WriteLine($"   List<int> 泛型参数: {intListType.GetGenericArguments()[0].Name}");

        // 类型检查
        Console.WriteLine($"   strings is List<string>: {strings is List<string>}"); // true
        Console.WriteLine($"   strings is List<int>: {strings is List<int>}"); // false

        // 【Java 无法做到的事情】
        // 1. 无法在运行时检查 List<String> 和 List<Integer> 的区别
        // 2. 无法创建泛型数组 new T[]
        // 3. 无法使用 instanceof 检查泛型类型
    }

    /// <summary>
    /// 泛型约束
    ///
    /// 【Java 对比】
    /// Java: <T extends Comparable<T>>
    /// C#:   where T : IComparable<T>
    /// </summary>
    private void GenericConstraints()
    {
        // 基本约束
        Console.WriteLine("   泛型约束示例:");

        // where T : struct - 值类型约束
        Console.WriteLine("   - where T : struct (值类型)");

        // where T : class - 引用类型约束
        Console.WriteLine("   - where T : class (引用类型)");

        // where T : new() - 无参构造函数约束
        Console.WriteLine("   - where T : new() (无参构造函数)");

        // where T : 基类名 - 基类约束
        Console.WriteLine("   - where T : BaseClass (基类约束)");

        // where T : 接口名 - 接口约束
        Console.WriteLine("   - where T : IComparable<T> (接口约束)");

        // where T : U - 类型参数约束
        Console.WriteLine("   - where T : U (类型参数约束)");

        // 示例：带约束的泛型方法
        var result = Max(10, 20);
        Console.WriteLine($"\n   Max(10, 20) = {result}");

        var result2 = Max("Hello", "World");
        Console.WriteLine($"   Max(\"Hello\", \"World\") = {result2}");
    }

    /// <summary>
    /// 带约束的泛型方法
    /// </summary>
    private T Max<T>(T a, T b) where T : IComparable<T>
    {
        return a.CompareTo(b) > 0 ? a : b;
    }

    /// <summary>
    /// 协变和逆变
    ///
    /// 【Java 对比】
    /// Java: ? extends T (协变), ? super T (逆变)
    /// C#:   out T (协变), in T (逆变)
    ///
    /// 【关键区别】
    /// C# 的协变和逆变只能用于接口和委托
    /// </summary>
    private void CovarianceAndContravariance()
    {
        // 协变 (out) - 返回类型可以从派生类转换为基类
        Console.WriteLine("   协变 (out):");
        Console.WriteLine("   - IEnumerable<out T>: 可以将 IEnumerable<string> 转换为 IEnumerable<object>");

        // 示例
        IEnumerable<string> strings = new List<string> { "Hello", "World" };
        IEnumerable<object> objects = strings; // 协变：string 是 object 的子类
        Console.WriteLine($"   strings 可以赋值给 objects: true");

        // 逆变 (in) - 参数类型可以从基类转换为派生类
        Console.WriteLine("\n   逆变 (in):");
        Console.WriteLine("   - Action<in T>: 可以将 Action<object> 转换为 Action<string>");

        // 示例
        Action<object> objectAction = obj => Console.WriteLine($"   处理: {obj}");
        Action<string> stringAction = objectAction; // 逆变：object 是 string 的基类
        stringAction("Hello");

        // 不变 (Invariant)
        Console.WriteLine("\n   不变 (Invariant):");
        Console.WriteLine("   - List<T>: 不能转换，类型必须完全匹配");
        Console.WriteLine("   - 这是为了保证类型安全");
    }

    /// <summary>
    /// 泛型方法
    ///
    /// 【Java 对比】
    /// Java: public <T> void print(T item) { ... }
    /// C#:   public void Print<T>(T item) { ... }
    /// </summary>
    private void GenericMethods()
    {
        // 基本泛型方法
        Print(42);
        Print("Hello");
        Print(3.14);

        // 带约束的泛型方法
        var numbers = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 };
        var sorted = Sort(numbers);
        Console.WriteLine($"\n   排序后: {string.Join(", ", sorted)}");

        // 泛型方法的类型推断
        var result = CreatePair("Hello", 42);
        Console.WriteLine($"   创建对: ({result.First}, {result.Second})");
    }

    private void Print<T>(T item)
    {
        Console.WriteLine($"   类型: {typeof(T).Name}, 值: {item}");
    }

    private T[] Sort<T>(T[] array) where T : IComparable<T>
    {
        var result = new T[array.Length];
        Array.Copy(array, result, array.Length);
        Array.Sort(result);
        return result;
    }

    private (T1 First, T2 Second) CreatePair<T1, T2>(T1 first, T2 second)
    {
        return (first, second);
    }

    /// <summary>
    /// 泛型类
    ///
    /// 【Java 对比】
    /// Java: public class Box<T> { private T value; }
    /// C#:   public class Box<T> { private T _value; }
    /// </summary>
    private void GenericClasses()
    {
        // 基本泛型类
        var intBox = new Box<int>(42);
        var stringBox = new Box<string>("Hello");

        Console.WriteLine($"   intBox: {intBox.Value}");
        Console.WriteLine($"   stringBox: {stringBox.Value}");

        // 带约束的泛型类
        var repository = new Repository<DeviceReading>();
        repository.Add(new DeviceReading { Temperature = 25.0, Pressure = 0.3, Speed = 1500 });
        repository.Add(new DeviceReading { Temperature = 30.0, Pressure = 0.4, Speed = 1800 });

        Console.WriteLine($"   仓库中有 {repository.Count} 条记录");

        // 泛型接口
        ILogger<DeviceReading> logger = new ConsoleLogger<DeviceReading>();
        logger.Log(repository.GetAll());
    }

    /// <summary>
    /// 实际应用 - 通用仓储模式
    /// </summary>
    private void PracticalExample()
    {
        Console.WriteLine("   通用仓储模式示例:");

        // 创建设备读数仓储
        var readingRepo = new InMemoryRepository<DeviceReading, int>();
        readingRepo.Add(new DeviceReading { Id = 1, Temperature = 25.0, Pressure = 0.3, Speed = 1500 });
        readingRepo.Add(new DeviceReading { Id = 2, Temperature = 30.0, Pressure = 0.4, Speed = 1800 });

        // 查询
        var allReadings = readingRepo.GetAll();
        Console.WriteLine($"   总记录数: {allReadings.Count()}");

        var reading1 = readingRepo.GetById(1);
        Console.WriteLine($"   ID=1 的记录: 温度={reading1?.Temperature}°C");

        // 创建传感器配置仓储
        var configRepo = new InMemoryRepository<SensorConfig, string>();
        configRepo.Add(new SensorConfig { Name = "温度传感器", MinValue = 15, MaxValue = 35 });
        configRepo.Add(new SensorConfig { Name = "压力传感器", MinValue = 0.1, MaxValue = 0.5 });

        var allConfigs = configRepo.GetAll();
        Console.WriteLine($"   配置数量: {allConfigs.Count()}");

        // 【Java 对比】
        // Java 需要为每种类型创建单独的仓储类
        // C# 可以使用泛型创建通用的仓储类
    }
}

// ========== 泛型类示例 ==========

/// <summary>
/// 简单的泛型盒子类
/// </summary>
public class Box<T>
{
    public T Value { get; }

    public Box(T value)
    {
        Value = value;
    }
}

/// <summary>
/// 泛型仓储接口
/// </summary>
public interface IRepository<T, TId> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(TId id);
    void Add(T entity);
    void Update(T entity);
    void Delete(TId id);
}

/// <summary>
/// 内存仓储实现
/// </summary>
public class InMemoryRepository<T, TId> : IRepository<T, TId> where T : class
{
    private readonly Dictionary<TId, T> _store = new();

    public IEnumerable<T> GetAll() => _store.Values;

    public T? GetById(TId id) => _store.TryGetValue(id, out var entity) ? entity : null;

    public void Add(T entity)
    {
        // 使用反射获取 Id 属性（简化示例）
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var id = (TId)idProperty.GetValue(entity)!;
            _store[id] = entity;
        }
    }

    public void Update(T entity) => Add(entity);

    public void Delete(TId id) => _store.Remove(id);
}

/// <summary>
/// 泛型日志接口
/// </summary>
public interface ILogger<T>
{
    void Log(T item);
    void Log(IEnumerable<T> items);
}

/// <summary>
/// 控制台日志实现
/// </summary>
public class ConsoleLogger<T> : ILogger<T>
{
    public void Log(T item)
    {
        Console.WriteLine($"   [LOG] {typeof(T).Name}: {item}");
    }

    public void Log(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Log(item);
        }
    }
}

// ========== 数据模型 ==========

public class DeviceReading
{
    public int Id { get; init; }
    public double Temperature { get; init; }
    public double Pressure { get; init; }
    public double Speed { get; init; }

    public override string ToString() =>
        $"温度={Temperature:F1}°C, 压力={Pressure:F3}MPa, 转速={Speed:F0}rpm";
}

public class SensorConfig
{
    public string Name { get; init; } = string.Empty;
    public double MinValue { get; init; }
    public double MaxValue { get; init; }

    public override string ToString() =>
        $"{Name}: [{MinValue}, {MaxValue}]";
}
