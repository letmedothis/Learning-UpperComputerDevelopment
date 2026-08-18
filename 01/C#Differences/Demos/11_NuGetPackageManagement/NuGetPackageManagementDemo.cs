namespace C_Differences.Demos._11_NuGetPackageManagement;

/// <summary>
/// NuGet 包管理对比 - Java Maven/Gradle vs C# NuGet
///
/// 【为什么需要了解包管理】
/// 包管理是日常开发的核心技能。从 Java 转到 C# 时，
/// 了解 NuGet 与 Maven/Gradle 的差异能帮你快速上手。
///
/// 【Java 对比总览】
/// ┌─────────────────┬─────────────────┬─────────────────┬─────────────────┐
/// │ 概念             │ Java Maven      │ Java Gradle     │ C# NuGet        │
/// ├─────────────────┼─────────────────┼─────────────────┼─────────────────┤
/// │ 配置文件         │ pom.xml         │ build.gradle    │ .csproj         │
/// │ 包仓库           │ Maven Central   │ Maven Central   │ NuGet.org       │
/// │ 依赖声明         │ <dependency>    │ implementation  │ PackageReference│
/// │ 构建工具         │ mvn             │ gradle          │ dotnet          │
/// │ 包缓存           │ ~/.m2/repository│ ~/.gradle/caches│ ~/.nuget/packages│
/// │ 锁定文件         │ 无              │ gradle.lockfile │ packages.lock.json│
/// └─────────────────┴─────────────────┴─────────────────┴─────────────────┘
///
/// 【关键区别】
/// 1. C# 的包管理集成在项目文件中，不需要单独的构建文件
/// 2. C# 使用 PackageReference 语法，比 Maven 更简洁
/// 3. C# 的 NuGet.org 是官方包仓库，类似 Maven Central
/// 4. C# 的 dotnet CLI 集成了包管理命令，比 Maven 更方便
///
/// 【常用命令速查】
/// <code>
/// # 添加包
/// dotnet add package Newtonsoft.Json
///
/// # 移除包
/// dotnet remove package Newtonsoft.Json
///
/// # 恢复包
/// dotnet restore
///
/// # 列出包
/// dotnet list package
///
/// # 更新包
/// dotnet outdated
/// </code>
///
/// 【学习建议】
/// 1. 先掌握基本的添加/移除/恢复命令
/// 2. 了解常用 NuGet 包及其 Java 等价物
/// 3. 理解版本管理和锁定机制
/// 4. 在实际项目中多使用 dotnet CLI
/// </summary>
public class NuGetPackageManagementDemo
{
    public void Demo()
    {
        Console.WriteLine("=== NuGet 包管理对比 ===\n");

        Console.WriteLine("1. 项目文件对比:");
        ProjectFileComparison();

        Console.WriteLine("\n2. 依赖声明对比:");
        DependencyDeclarationComparison();

        Console.WriteLine("\n3. 常用 NuGet 包:");
        CommonNuGetPackages();

        Console.WriteLine("\n4. NuGet 命令对比:");
        NuGetCommandsComparison();

        Console.WriteLine("\n5. 版本管理:");
        VersionManagement();

        Console.WriteLine("\n6. 包恢复和还原:");
        PackageRestore();

        Console.WriteLine("\n7. 实际应用 - 项目文件分析:");
        PracticalExample();
    }

    /// <summary>
    /// 项目文件对比
    ///
    /// 【Java 对比】
    /// Java Maven: pom.xml
    /// Java Gradle: build.gradle
    /// C# .NET: .csproj
    /// </summary>
    private void ProjectFileComparison()
    {
        Console.WriteLine("   Java Maven (pom.xml):");
        Console.WriteLine("   ```xml");
        Console.WriteLine("   <project>");
        Console.WriteLine("     <modelVersion>4.0.0</modelVersion>");
        Console.WriteLine("     <groupId>com.example</groupId>");
        Console.WriteLine("     <artifactId>my-app</artifactId>");
        Console.WriteLine("     <version>1.0.0</version>");
        Console.WriteLine("     <dependencies>");
        Console.WriteLine("       <dependency>");
        Console.WriteLine("         <groupId>org.springframework.boot</groupId>");
        Console.WriteLine("         <artifactId>spring-boot-starter</artifactId>");
        Console.WriteLine("         <version>3.0.0</version>");
        Console.WriteLine("       </dependency>");
        Console.WriteLine("     </dependencies>");
        Console.WriteLine("   </project>");
        Console.WriteLine("   ```");

        Console.WriteLine("\n   C# .NET (.csproj):");
        Console.WriteLine("   ```xml");
        Console.WriteLine("   <Project Sdk=\"Microsoft.NET.Sdk\">");
        Console.WriteLine("     <PropertyGroup>");
        Console.WriteLine("       <OutputType>Exe</OutputType>");
        Console.WriteLine("       <TargetFramework>net8.0</TargetFramework>");
        Console.WriteLine("     </PropertyGroup>");
        Console.WriteLine("     <ItemGroup>");
        Console.WriteLine("       <PackageReference Include=\"Microsoft.Extensions.Hosting\"");
        Console.WriteLine("                        Version=\"8.0.0\" />");
        Console.WriteLine("     </ItemGroup>");
        Console.WriteLine("   </Project>");
        Console.WriteLine("   ```");

        Console.WriteLine("\n   【关键区别】");
        Console.WriteLine("   - C# 使用 PackageReference，更简洁");
        Console.WriteLine("   - C# 不需要 groupId，使用命名空间");
        Console.WriteLine("   - C# 的 TargetFramework 指定目标框架");
    }

    /// <summary>
    /// 依赖声明对比
    /// </summary>
    private void DependencyDeclarationComparison()
    {
        Console.WriteLine("   Java Maven 依赖:");
        Console.WriteLine("   ```xml");
        Console.WriteLine("   <dependency>");
        Console.WriteLine("     <groupId>org.springframework.boot</groupId>");
        Console.WriteLine("     <artifactId>spring-boot-starter-web</artifactId>");
        Console.WriteLine("     <version>3.0.0</version>");
        Console.WriteLine("   </dependency>");
        Console.WriteLine("   ```");

        Console.WriteLine("\n   C# NuGet 依赖:");
        Console.WriteLine("   ```xml");
        Console.WriteLine("   <PackageReference Include=\"Microsoft.AspNetCore.Mvc\"");
        Console.WriteLine("                    Version=\"8.0.0\" />");
        Console.WriteLine("   ```");

        Console.WriteLine("\n   Java Gradle 依赖:");
        Console.WriteLine("   ```groovy");
        Console.WriteLine("   implementation 'org.springframework.boot:spring-boot-starter-web:3.0.0'");
        Console.WriteLine("   ```");

        Console.WriteLine("\n   【关键区别】");
        Console.WriteLine("   - C# 的 Include 属性同时包含 groupId 和 artifactId");
        Console.WriteLine("   - C# 的版本号使用语义化版本 (SemVer)");
    }

    /// <summary>
    /// 常用 NuGet 包
    /// </summary>
    private void CommonNuGetPackages()
    {
        Console.WriteLine("   常用 NuGet 包及其 Java 等价物:");

        var packages = new[]
        {
            ("Microsoft.Extensions.Hosting", "Spring Boot", "依赖注入、配置、日志"),
            ("Microsoft.Extensions.DependencyInjection", "Spring IoC", "依赖注入容器"),
            ("Microsoft.Extensions.Configuration", "Spring Config", "配置管理"),
            ("Microsoft.Extensions.Logging", "SLF4J/Log4j", "日志框架"),
            ("Newtonsoft.Json", "Jackson", "JSON 序列化"),
            ("System.Text.Json", "Jackson", "JSON 序列化（内置）"),
            ("Dapper", "MyBatis", "轻量级 ORM"),
            ("Microsoft.EntityFrameworkCore", "Hibernate/JPA", "ORM 框架"),
            ("xunit", "JUnit", "单元测试框架"),
            ("Moxy", "Mockito", "Mock 框架"),
            ("AutoMapper", "MapStruct", "对象映射"),
            ("Serilog", "Logback", "结构化日志"),
            ("Polly", "Resilience4j", "熔断、重试"),
            ("MediatR", "Spring Events", "中介者模式"),
            ("FluentValidation", "Hibernate Validator", "数据验证")
        };

        foreach (var (nuget, java, description) in packages)
        {
            Console.WriteLine($"   - {nuget,-40} ← {java,-20} ({description})");
        }
    }

    /// <summary>
    /// NuGet 命令对比
    /// </summary>
    private void NuGetCommandsComparison()
    {
        Console.WriteLine("   常用命令对比:");

        var commands = new[]
        {
            ("添加包", "NuGet", "dotnet add package <包名>"),
            ("", "Maven", "mvn dependency:resolve"),
            ("", "Gradle", "gradle dependencies"),
            ("移除包", "NuGet", "dotnet remove package <包名>"),
            ("", "Maven", "手动编辑 pom.xml"),
            ("", "Gradle", "手动编辑 build.gradle"),
            ("恢复包", "NuGet", "dotnet restore"),
            ("", "Maven", "mvn dependency:resolve"),
            ("", "Gradle", "gradle build"),
            ("列出包", "NuGet", "dotnet list package"),
            ("", "Maven", "mvn dependency:tree"),
            ("", "Gradle", "gradle dependencies"),
            ("更新包", "NuGet", "dotnet outdated"),
            ("", "Maven", "mvn versions:display-dependency-updates"),
            ("", "Gradle", "gradle dependencyUpdates")
        };

        foreach (var (action, tool, command) in commands)
        {
            if (!string.IsNullOrEmpty(action))
            {
                Console.WriteLine($"\n   {action}:");
            }
            Console.WriteLine($"     {tool,-10}: {command}");
        }
    }

    /// <summary>
    /// 版本管理
    /// </summary>
    private void VersionManagement()
    {
        Console.WriteLine("   版本管理对比:");

        Console.WriteLine("\n   1. 语义化版本 (SemVer):");
        Console.WriteLine("      主版本.次版本.修订号 (例如: 1.2.3)");
        Console.WriteLine("      - 主版本: 不兼容的 API 更改");
        Console.WriteLine("      - 次版本: 向后兼容的新功能");
        Console.WriteLine("      - 修订号: 向后兼容的 bug 修复");

        Console.WriteLine("\n   2. 版本范围:");
        Console.WriteLine("      NuGet:");
        Console.WriteLine("        Version=\"1.0.0\"        // 精确版本");
        Console.WriteLine("        Version=\"1.0.*\"        // 通配符");
        Console.WriteLine("        Version=\"[1.0,2.0)\"    // 范围");
        Console.WriteLine("      Maven:");
        Console.WriteLine("        <version>1.0.0</version>        // 精确版本");
        Console.WriteLine("        <version>[1.0,2.0)</version>    // 范围");

        Console.WriteLine("\n   3. 锁定版本:");
        Console.WriteLine("      NuGet: 使用 packages.lock.json");
        Console.WriteLine("      Maven: 使用 mvn dependency:resolve");
        Console.WriteLine("      Gradle: 使用 gradle.lockfile");
    }

    /// <summary>
    /// 包恢复和还原
    /// </summary>
    private void PackageRestore()
    {
        Console.WriteLine("   包恢复机制对比:");

        Console.WriteLine("\n   NuGet:");
        Console.WriteLine("   - 自动恢复: 构建时自动下载缺失的包");
        Console.WriteLine("   - 手动恢复: dotnet restore");
        Console.WriteLine("   - 缓存位置: ~/.nuget/packages");
        Console.WriteLine("   - 全局包: ~/.nuget/packages (所有项目共享)");

        Console.WriteLine("\n   Maven:");
        Console.WriteLine("   - 自动恢复: 构建时自动下载缺失的依赖");
        Console.WriteLine("   - 手动恢复: mvn dependency:resolve");
        Console.WriteLine("   - 缓存位置: ~/.m2/repository");
        Console.WriteLine("   - 全局仓库: ~/.m2/repository (所有项目共享)");

        Console.WriteLine("\n   Gradle:");
        Console.WriteLine("   - 自动恢复: 构建时自动下载缺失的依赖");
        Console.WriteLine("   - 手动恢复: gradle build");
        Console.WriteLine("   - 缓存位置: ~/.gradle/caches");
        Console.WriteLine("   - 全局缓存: ~/.gradle/caches (所有项目共享)");
    }

    /// <summary>
    /// 实际应用 - 项目文件分析
    /// </summary>
    private void PracticalExample()
    {
        Console.WriteLine("   实际项目文件分析:");

        Console.WriteLine("\n   示例 1: 控制台应用");
        Console.WriteLine("   ```xml");
        Console.WriteLine("   <Project Sdk=\"Microsoft.NET.Sdk\">");
        Console.WriteLine("     <PropertyGroup>");
        Console.WriteLine("       <OutputType>Exe</OutputType>");
        Console.WriteLine("       <TargetFramework>net8.0</TargetFramework>");
        Console.WriteLine("       <Nullable>enable</Nullable>");
        Console.WriteLine("       <ImplicitUsings>enable</ImplicitUsings>");
        Console.WriteLine("     </PropertyGroup>");
        Console.WriteLine("     <ItemGroup>");
        Console.WriteLine("       <PackageReference Include=\"Microsoft.Extensions.Hosting\" Version=\"8.0.0\" />");
        Console.WriteLine("       <PackageReference Include=\"Serilog\" Version=\"3.1.1\" />");
        Console.WriteLine("     </ItemGroup>");
        Console.WriteLine("   </Project>");
        Console.WriteLine("   ```");

        Console.WriteLine("\n   示例 2: 类库项目");
        Console.WriteLine("   ```xml");
        Console.WriteLine("   <Project Sdk=\"Microsoft.NET.Sdk\">");
        Console.WriteLine("     <PropertyGroup>");
        Console.WriteLine("       <TargetFramework>net8.0</TargetFramework>");
        Console.WriteLine("       <Nullable>enable</Nullable>");
        Console.WriteLine("     </PropertyGroup>");
        Console.WriteLine("     <ItemGroup>");
        Console.WriteLine("       <PackageReference Include=\"Newtonsoft.Json\" Version=\"13.0.3\" />");
        Console.WriteLine("     </ItemGroup>");
        Console.WriteLine("   </Project>");
        Console.WriteLine("   ```");

        Console.WriteLine("\n   示例 3: 解决方案文件 (.sln)");
        Console.WriteLine("   解决方案文件组织多个项目:");
        Console.WriteLine("   ```");
        Console.WriteLine("   MySolution.sln");
        Console.WriteLine("   ├── src/");
        Console.WriteLine("   │   ├── MyApp.Core/");
        Console.WriteLine("   │   │   └── MyApp.Core.csproj");
        Console.WriteLine("   │   └── MyApp.App/");
        Console.WriteLine("   │       └── MyApp.App.csproj");
        Console.WriteLine("   └── tests/");
        Console.WriteLine("       └── MyApp.Tests/");
        Console.WriteLine("           └── MyApp.Tests.csproj");
        Console.WriteLine("   ```");
    }
}
