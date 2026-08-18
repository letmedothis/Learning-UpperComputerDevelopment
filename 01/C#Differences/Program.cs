using C_Differences.Demos._01_Property;
using C_Differences.Demos._02_NullableReference;
using C_Differences.Demos._03_Records;
using C_Differences.Demos._04_DelegatesEvents;
using C_Differences.Demos._05_LINQ;
using C_Differences.Demos._06_AsyncAwait;
using C_Differences.Demos._07_Disposable;
using C_Differences.Demos._08_ConfigLoggingDI;
using C_Differences.Demos._09_PatternMatching;
using C_Differences.Demos._10_CommonTraps;
using C_Differences.Demos._11_NuGetPackageManagement;
using C_Differences.Demos._12_Generics;

// 学习 C# Property
Console.WriteLine("=== C# Property 学习 ===");

// 1. 基本的 Property 示例
var person = new Person
{
    Name = "张三",
    Age = 25
};

Console.WriteLine($"姓名: {person.Name}, 年龄: {person.Age}");

// 2. 只读 Property
var product = new Product("笔记本电脑", 5999.99m);
Console.WriteLine($"产品: {product.Name}, 价格: {product.Price}");

// 3. 带验证的 Property
var account = new Account();
account.Balance = 1000; // 设置值
Console.WriteLine($"账户余额: {account.Balance}");

try
{
    account.Balance = -100; // 这会抛出异常
}
catch (ArgumentException ex)
{
    Console.WriteLine($"错误: {ex.Message}");
}

// 4. 计算属性
var rectangle = new Rectangle(5, 3);
Console.WriteLine($"矩形面积: {rectangle.Area}");

// 5. 自动属性与手动属性的区别演示
var config = new Config();
config.Timeout = 30;
Console.WriteLine($"超时设置: {config.Timeout}ms");

// 可空引用类型演示
Console.WriteLine("\n=== C# 可空引用类型学习 ===");
var nullableDemo = new NullableReferenceDemo();
nullableDemo.Demo();

// record 类型演示
Console.WriteLine("\n=== C# record 类型学习 ===");
var recordDemo = new RecordDemo();
recordDemo.Demo();

// 委托、事件、Lambda 演示
Console.WriteLine("\n=== C# 委托、事件、Lambda 学习 ===");
var delegateDemo = new DelegateEventLambdaDemo();
delegateDemo.Demo();

// LINQ 演示
Console.WriteLine("\n=== C# LINQ 学习 ===");
var linqDemo = new LinqDemo();
linqDemo.Demo();

// Task 和 async/await 演示
Console.WriteLine("\n=== C# Task 和 async/await 学习 ===");
var taskDemo = new TaskAsyncDemo();
await taskDemo.DemoAsync();

// CancellationToken 演示
Console.WriteLine("\n=== C# CancellationToken 学习 ===");
var cancellationDemo = new CancellationDemo();
await cancellationDemo.DemoAsync();

// IDisposable 演示
Console.WriteLine("\n=== C# IDisposable 学习 ===");
var disposableDemo = new DisposableDemo();
disposableDemo.Demo();

// 配置、日志、依赖注入 演示
Console.WriteLine("\n=== C# 配置、日志、依赖注入 学习 ===");
var configLoggingDIDemo = new ConfigLoggingDIDemo();
configLoggingDIDemo.Demo();

// 模式匹配 演示
Console.WriteLine("\n=== C# 模式匹配学习 ===");
var patternMatchingDemo = new PatternMatchingDemo();
patternMatchingDemo.Demo();

// 常见陷阱 演示
Console.WriteLine("\n=== Java 开发者常见陷阱 ===");
var commonTrapsDemo = new CommonTrapsDemo();
commonTrapsDemo.Demo();

// NuGet 包管理 演示
Console.WriteLine("\n=== NuGet 包管理对比 ===");
var nugetDemo = new NuGetPackageManagementDemo();
nugetDemo.Demo();

// 泛型差异 演示
Console.WriteLine("\n=== 泛型差异学习 ===");
var genericsDemo = new GenericsDemo();
genericsDemo.Demo();
