# WPF Device Monitor V1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a beginner-friendly WPF V1 in `02` that displays fake device data every second, keeps the latest 20 samples, uses Dispatcher safely, and stops collection before the window closes.

**Architecture:** Use three projects: a WPF App, a WPF-free Core library, and xUnit Tests. Core owns immutable readings, evaluation rules, fake generation, the 20-item buffer, and the cancellable acquisition loop; `MainWindow` owns the V1 binding snapshot, button event handlers, Dispatcher handoff, and closing handshake without introducing MVVM.

**Tech Stack:** C# 14, .NET 10, WPF, XAML, xUnit 2.9.3, Microsoft.NET.Test.Sdk 17.12.0.

---

## File map

### Solution and documentation

- Create `02/ProductionLineMonitor.slnx`: contains all three projects.
- Create `02/.gitignore`: ignores only generated files under week 2.
- Create `02/README.md`: beginner guide, architecture, commands, acceptance notes, and screenshots.

### Core project

- Create `02/ProductionLineMonitor.Core/ProductionLineMonitor.Core.csproj`: WPF-free `net10.0` class library.
- Create `02/ProductionLineMonitor.Core/Models/MetricLevel.cs`: `Normal`, `Warning`, `Alarm`.
- Create `02/ProductionLineMonitor.Core/Models/DeviceReading.cs`: immutable sample with values, production, and levels.
- Create `02/ProductionLineMonitor.Core/Services/ReadingEvaluator.cs`: pure threshold functions and overall severity.
- Create `02/ProductionLineMonitor.Core/Services/RecentReadingBuffer.cs`: newest-first fixed-capacity buffer.
- Create `02/ProductionLineMonitor.Core/Services/FakeDataGenerator.cs`: generates one sample and owns production count.
- Create `02/ProductionLineMonitor.Core/Services/AcquisitionService.cs`: background periodic loop with cooperative cancellation.

### App project

- Create `02/ProductionLineMonitor.App/ProductionLineMonitor.App.csproj`: `net10.0-windows` WPF executable.
- Create `02/ProductionLineMonitor.App/Models/OperatingState.cs`: stopped/running/stopping UI state.
- Create `02/ProductionLineMonitor.App/Models/MetricCardItem.cs`: one immutable metric card binding item.
- Create `02/ProductionLineMonitor.App/Models/DashboardState.cs`: immutable DataContext snapshot.
- Create `02/ProductionLineMonitor.App/Converters/MetricLevelToBrushConverter.cs`: numeric status to theme brush.
- Create `02/ProductionLineMonitor.App/Converters/OperatingStateToBrushConverter.cs`: machine state to theme brush.
- Create `02/ProductionLineMonitor.App/Themes/Colors.xaml`: color and brush resources.
- Create `02/ProductionLineMonitor.App/Themes/Controls.xaml`: button, badge, card, DataGrid styles and card DataTemplate.
- Modify generated `02/ProductionLineMonitor.App/App.xaml`: merge dictionaries and register converters.
- Modify generated `02/ProductionLineMonitor.App/MainWindow.xaml`: responsive dashboard.
- Modify generated `02/ProductionLineMonitor.App/MainWindow.xaml.cs`: V1 orchestration and lifecycle.

### Test project

- Create `02/ProductionLineMonitor.Tests/ProductionLineMonitor.Tests.csproj`: xUnit test project referencing Core.
- Create `02/ProductionLineMonitor.Tests/Services/ReadingEvaluatorTests.cs`.
- Create `02/ProductionLineMonitor.Tests/Services/RecentReadingBufferTests.cs`.
- Create `02/ProductionLineMonitor.Tests/Services/FakeDataGeneratorTests.cs`.
- Create `02/ProductionLineMonitor.Tests/Services/AcquisitionServiceTests.cs`.

## Task 1: Scaffold the isolated week-2 solution

**Files:**
- Create: `02/ProductionLineMonitor.slnx`
- Create: `02/.gitignore`
- Create: `02/ProductionLineMonitor.Core/ProductionLineMonitor.Core.csproj`
- Create: `02/ProductionLineMonitor.App/ProductionLineMonitor.App.csproj`
- Create: `02/ProductionLineMonitor.Tests/ProductionLineMonitor.Tests.csproj`

- [ ] **Step 1: Generate the solution and three projects**

Run from the repository root:

```powershell
dotnet new sln -n ProductionLineMonitor --format slnx -o .\02
dotnet new classlib -n ProductionLineMonitor.Core -o .\02\ProductionLineMonitor.Core -f net10.0
dotnet new wpf -n ProductionLineMonitor.App -o .\02\ProductionLineMonitor.App -f net10.0
dotnet new xunit -n ProductionLineMonitor.Tests -o .\02\ProductionLineMonitor.Tests -f net10.0
dotnet sln .\02\ProductionLineMonitor.slnx add .\02\ProductionLineMonitor.Core\ProductionLineMonitor.Core.csproj
dotnet sln .\02\ProductionLineMonitor.slnx add .\02\ProductionLineMonitor.App\ProductionLineMonitor.App.csproj
dotnet sln .\02\ProductionLineMonitor.slnx add .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj
dotnet add .\02\ProductionLineMonitor.App\ProductionLineMonitor.App.csproj reference .\02\ProductionLineMonitor.Core\ProductionLineMonitor.Core.csproj
dotnet add .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj reference .\02\ProductionLineMonitor.Core\ProductionLineMonitor.Core.csproj
```

Expected: all commands exit `0`; the App project contains `<UseWPF>true</UseWPF>` and targets `net10.0-windows`.

- [ ] **Step 2: Pin the existing repository test package versions**

Replace `ProductionLineMonitor.Tests.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\ProductionLineMonitor.Core\ProductionLineMonitor.Core.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Remove generated placeholder code and add week-local ignores**

Delete:

```text
02/ProductionLineMonitor.Core/Class1.cs
02/ProductionLineMonitor.Tests/UnitTest1.cs
```

Create `02/.gitignore`:

```gitignore
**/bin/
**/obj/
.vs/
TestResults/
```

- [ ] **Step 4: Restore and build the empty scaffold**

Run:

```powershell
dotnet restore .\02\ProductionLineMonitor.slnx
dotnet build .\02\ProductionLineMonitor.slnx -c Release --no-restore -warnaserror
```

Expected: both commands exit `0`, with `0 Warning(s)` and `0 Error(s)`.

- [ ] **Step 5: Commit only the scaffold**

```powershell
git add -- 02/.gitignore 02/ProductionLineMonitor.slnx 02/ProductionLineMonitor.Core 02/ProductionLineMonitor.App 02/ProductionLineMonitor.Tests
git commit -m "build: scaffold WPF monitor solution"
```

## Task 2: Implement metric evaluation with boundary-first TDD

**Files:**
- Create: `02/ProductionLineMonitor.Core/Models/MetricLevel.cs`
- Create: `02/ProductionLineMonitor.Core/Models/DeviceReading.cs`
- Create: `02/ProductionLineMonitor.Core/Services/ReadingEvaluator.cs`
- Create: `02/ProductionLineMonitor.Tests/Services/ReadingEvaluatorTests.cs`

- [ ] **Step 1: Write failing threshold tests**

Create tests that call the desired API before production types exist:

```csharp
using ProductionLineMonitor.Core.Models;
using ProductionLineMonitor.Core.Services;

namespace ProductionLineMonitor.Tests.Services;

public sealed class ReadingEvaluatorTests
{
    [Theory]
    [InlineData(9.9, MetricLevel.Alarm)]
    [InlineData(10, MetricLevel.Warning)]
    [InlineData(14.9, MetricLevel.Warning)]
    [InlineData(15, MetricLevel.Normal)]
    [InlineData(35, MetricLevel.Normal)]
    [InlineData(35.1, MetricLevel.Warning)]
    [InlineData(40, MetricLevel.Warning)]
    [InlineData(40.1, MetricLevel.Alarm)]
    public void EvaluateTemperature_ReturnsExpectedLevel(double value, MetricLevel expected) =>
        Assert.Equal(expected, ReadingEvaluator.EvaluateTemperature(value));

    [Theory]
    [InlineData(0.049, MetricLevel.Alarm)]
    [InlineData(0.05, MetricLevel.Warning)]
    [InlineData(0.099, MetricLevel.Warning)]
    [InlineData(0.1, MetricLevel.Normal)]
    [InlineData(0.5, MetricLevel.Normal)]
    [InlineData(0.501, MetricLevel.Warning)]
    [InlineData(0.6, MetricLevel.Warning)]
    [InlineData(0.601, MetricLevel.Alarm)]
    public void EvaluatePressure_ReturnsExpectedLevel(double value, MetricLevel expected) =>
        Assert.Equal(expected, ReadingEvaluator.EvaluatePressure(value));

    [Theory]
    [InlineData(499, MetricLevel.Alarm)]
    [InlineData(500, MetricLevel.Warning)]
    [InlineData(999, MetricLevel.Warning)]
    [InlineData(1000, MetricLevel.Normal)]
    [InlineData(2000, MetricLevel.Normal)]
    [InlineData(2001, MetricLevel.Warning)]
    [InlineData(2500, MetricLevel.Warning)]
    [InlineData(2501, MetricLevel.Alarm)]
    public void EvaluateSpeed_ReturnsExpectedLevel(double value, MetricLevel expected) =>
        Assert.Equal(expected, ReadingEvaluator.EvaluateSpeed(value));

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void EvaluateTemperature_WithNonFiniteValue_ReturnsAlarm(double value) =>
        Assert.Equal(MetricLevel.Alarm, ReadingEvaluator.EvaluateTemperature(value));

    [Fact]
    public void GetOverallLevel_ReturnsMostSevereLevel() =>
        Assert.Equal(
            MetricLevel.Alarm,
            ReadingEvaluator.GetOverallLevel(MetricLevel.Normal, MetricLevel.Alarm, MetricLevel.Warning));

    [Fact]
    public void GetOverallLevel_WithNoLevels_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => ReadingEvaluator.GetOverallLevel());
}
```

- [ ] **Step 2: Run the focused test and verify RED**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~ReadingEvaluatorTests
```

Expected: compile failure because `MetricLevel` and `ReadingEvaluator` do not exist.

- [ ] **Step 3: Add the minimal enum, reading record, and evaluator**

Implement `MetricLevel` in severity order and evaluator methods with finite checks, normal interval first, warning interval second, alarm otherwise. Define this API exactly:

```csharp
public enum MetricLevel { Normal = 0, Warning = 1, Alarm = 2 }

public sealed record DeviceReading(
    DateTime Timestamp,
    double Temperature,
    double Pressure,
    double Speed,
    int Production,
    MetricLevel TemperatureLevel,
    MetricLevel PressureLevel,
    MetricLevel SpeedLevel,
    MetricLevel OverallLevel);

public static class ReadingEvaluator
{
    public static MetricLevel EvaluateTemperature(double value);
    public static MetricLevel EvaluatePressure(double value);
    public static MetricLevel EvaluateSpeed(double value);
    public static MetricLevel GetOverallLevel(params MetricLevel[] levels);
}
```

`GetOverallLevel` must throw `ArgumentException` for an empty array and return the maximum enum severity otherwise.

- [ ] **Step 4: Run focused and complete tests to verify GREEN**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~ReadingEvaluatorTests
dotnet test .\02\ProductionLineMonitor.slnx -c Release
```

Expected: all evaluator cases pass; the complete suite has `0` failures.

- [ ] **Step 5: Commit evaluator behavior**

```powershell
git add -- 02/ProductionLineMonitor.Core/Models 02/ProductionLineMonitor.Core/Services/ReadingEvaluator.cs 02/ProductionLineMonitor.Tests/Services/ReadingEvaluatorTests.cs
git commit -m "feat: evaluate device metric status"
```

## Task 3: Implement the latest-20 buffer with TDD

**Files:**
- Create: `02/ProductionLineMonitor.Core/Services/RecentReadingBuffer.cs`
- Create: `02/ProductionLineMonitor.Tests/Services/RecentReadingBufferTests.cs`

- [ ] **Step 1: Write failing capacity, order, and clear tests**

Use a helper that creates readings with `Production` equal to the sequence number. Test capacities `1`, `20`, `21`, and `25`; assert newest-first ordering and that `Clear()` leaves an empty snapshot.

```csharp
[Fact]
public void Add_WhenMoreThanCapacity_KeepsNewestTwentyInNewestFirstOrder()
{
    var buffer = new RecentReadingBuffer(20);
    for (var index = 1; index <= 25; index++) buffer.Add(CreateReading(index));

    Assert.Equal(20, buffer.Count);
    Assert.Equal(Enumerable.Range(6, 20).Reverse(), buffer.Snapshot.Select(x => x.Production));
}

[Fact]
public void Snapshot_CannotMutateInternalStorage()
{
    var buffer = new RecentReadingBuffer(20);
    buffer.Add(CreateReading(1));
    var snapshot = buffer.Snapshot;

    buffer.Add(CreateReading(2));

    Assert.Single(snapshot);
    Assert.Equal(2, buffer.Count);
}

[Fact]
public void Clear_RemovesAllReadings()
{
    var buffer = new RecentReadingBuffer(20);
    buffer.Add(CreateReading(1));
    buffer.Clear();
    Assert.Empty(buffer.Snapshot);
}
```

- [ ] **Step 2: Run focused tests and verify RED**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~RecentReadingBufferTests
```

Expected: compile failure because `RecentReadingBuffer` does not exist.

- [ ] **Step 3: Implement the fixed-capacity buffer**

Define:

```csharp
public sealed class RecentReadingBuffer
{
    public RecentReadingBuffer(int capacity = 20);
    public int Count { get; }
    public IReadOnlyList<DeviceReading> Snapshot { get; }
    public void Add(DeviceReading reading);
    public void Clear();
}
```

Reject `capacity <= 0` with `ArgumentOutOfRangeException`. Insert at index `0`, remove at index `capacity` while over capacity, and return `ToArray()` from `Snapshot` so callers cannot observe later mutation.

- [ ] **Step 4: Run focused and complete tests to verify GREEN**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~RecentReadingBufferTests
dotnet test .\02\ProductionLineMonitor.slnx -c Release
```

Expected: all buffer tests and the complete suite pass.

- [ ] **Step 5: Commit the buffer**

```powershell
git add -- 02/ProductionLineMonitor.Core/Services/RecentReadingBuffer.cs 02/ProductionLineMonitor.Tests/Services/RecentReadingBufferTests.cs
git commit -m "feat: retain latest twenty readings"
```

## Task 4: Implement fake sample generation with TDD

**Files:**
- Create: `02/ProductionLineMonitor.Core/Services/FakeDataGenerator.cs`
- Create: `02/ProductionLineMonitor.Tests/Services/FakeDataGeneratorTests.cs`

- [ ] **Step 1: Write failing range, production, reset, and level tests**

```csharp
[Fact]
public void Generate_CreatesValuesWithinSimulationRanges()
{
    var generator = new FakeDataGenerator(new Random(20260817));
    var readings = Enumerable.Range(0, 100).Select(_ => generator.Generate()).ToArray();

    Assert.All(readings, reading =>
    {
        Assert.InRange(reading.Temperature, 5, 45);
        Assert.InRange(reading.Pressure, 0.01, 0.69);
        Assert.InRange(reading.Speed, 300, 2700);
    });
}

[Fact]
public void Generate_IncreasesProductionAndResetStartsAgainFromZero()
{
    var generator = new FakeDataGenerator(new Random(7));
    var first = generator.Generate();
    var second = generator.Generate();
    Assert.True(second.Production > first.Production);

    generator.Reset();
    var afterReset = generator.Generate();
    Assert.InRange(afterReset.Production, 1, 5);
}

[Fact]
public void Generate_AssignsLevelsUsingReadingEvaluator()
{
    var generator = new FakeDataGenerator(new Random(9));
    var reading = generator.Generate();
    Assert.Equal(ReadingEvaluator.EvaluateTemperature(reading.Temperature), reading.TemperatureLevel);
    Assert.Equal(ReadingEvaluator.EvaluatePressure(reading.Pressure), reading.PressureLevel);
    Assert.Equal(ReadingEvaluator.EvaluateSpeed(reading.Speed), reading.SpeedLevel);
}
```

- [ ] **Step 2: Run focused tests and verify RED**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~FakeDataGeneratorTests
```

Expected: compile failure because `FakeDataGenerator` does not exist.

- [ ] **Step 3: Implement one-sample generation**

Define:

```csharp
public sealed class FakeDataGenerator
{
    public FakeDataGenerator(Random? random = null);
    public int Production { get; }
    public DeviceReading Generate();
    public void Reset();
}
```

Use the injected `Random`, generate temperature `[5,45)`, pressure `[0.01,0.69)`, speed `[300,2700)`, and add `Random.Next(1, 6)` to production. Build all levels through `ReadingEvaluator` and timestamp with `DateTime.Now`.

- [ ] **Step 4: Run focused and complete tests to verify GREEN**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~FakeDataGeneratorTests
dotnet test .\02\ProductionLineMonitor.slnx -c Release
```

Expected: generator tests and complete suite pass.

- [ ] **Step 5: Commit the generator**

```powershell
git add -- 02/ProductionLineMonitor.Core/Services/FakeDataGenerator.cs 02/ProductionLineMonitor.Tests/Services/FakeDataGeneratorTests.cs
git commit -m "feat: generate simulated device readings"
```

## Task 5: Implement cancellable background acquisition with TDD

**Files:**
- Create: `02/ProductionLineMonitor.Core/Services/AcquisitionService.cs`
- Create: `02/ProductionLineMonitor.Tests/Services/AcquisitionServiceTests.cs`

- [ ] **Step 1: Write failing callback and cancellation tests**

Use a 10 ms interval so tests are fast. Signal the first sample with `TaskCompletionSource` rather than sleeping for an arbitrary long duration.

```csharp
[Fact]
public async Task RunAsync_PublishesReadingsUntilCancelled()
{
    var service = new AcquisitionService(new FakeDataGenerator(new Random(1)), TimeSpan.FromMilliseconds(10));
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
    var firstReading = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var count = 0;

    var task = service.RunAsync(_ =>
    {
        Interlocked.Increment(ref count);
        firstReading.TrySetResult();
        return Task.CompletedTask;
    }, cts.Token);

    await firstReading.Task;
    cts.Cancel();
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.True(count >= 1);
}

[Fact]
public async Task RunAsync_AfterCancellation_DoesNotPublishMoreReadings()
{
    var service = new AcquisitionService(new FakeDataGenerator(new Random(2)), TimeSpan.FromMilliseconds(10));
    using var cts = new CancellationTokenSource();
    var firstReading = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var count = 0;
    var task = service.RunAsync(_ =>
    {
        Interlocked.Increment(ref count);
        firstReading.TrySetResult();
        return Task.CompletedTask;
    }, cts.Token);

    await firstReading.Task;
    cts.Cancel();
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    var countAfterCancel = count;
    await Task.Delay(40);
    Assert.Equal(countAfterCancel, count);
}
```

Also test that zero/negative intervals throw `ArgumentOutOfRangeException` and a null callback throws `ArgumentNullException`.

- [ ] **Step 2: Run focused tests and verify RED**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~AcquisitionServiceTests
```

Expected: compile failure because `AcquisitionService` does not exist.

- [ ] **Step 3: Implement the background loop**

Define:

```csharp
public sealed class AcquisitionService
{
    public AcquisitionService(FakeDataGenerator generator, TimeSpan? interval = null);
    public Task RunAsync(Func<DeviceReading, Task> onReading, CancellationToken cancellationToken);
}
```

`RunAsync` must return a `Task.Run` worker. Inside it, check cancellation, generate one reading, await the callback, then await `Task.Delay(_interval, cancellationToken)`. Use `ConfigureAwait(false)` inside Core. Do not catch cancellation in Core; the window decides that expected cancellation is not an error.

- [ ] **Step 4: Run focused and complete tests to verify GREEN**

```powershell
dotnet test .\02\ProductionLineMonitor.Tests\ProductionLineMonitor.Tests.csproj -c Release --filter FullyQualifiedName~AcquisitionServiceTests
dotnet test .\02\ProductionLineMonitor.slnx -c Release
```

Expected: cancellation completes in under the one-second test timeout and all tests pass.

- [ ] **Step 5: Commit acquisition behavior**

```powershell
git add -- 02/ProductionLineMonitor.Core/Services/AcquisitionService.cs 02/ProductionLineMonitor.Tests/Services/AcquisitionServiceTests.cs
git commit -m "feat: add cancellable acquisition loop"
```

## Task 6: Build the WPF presentation resources and binding snapshots

**Files:**
- Create: `02/ProductionLineMonitor.App/Models/OperatingState.cs`
- Create: `02/ProductionLineMonitor.App/Models/MetricCardItem.cs`
- Create: `02/ProductionLineMonitor.App/Models/DashboardState.cs`
- Create: `02/ProductionLineMonitor.App/Converters/MetricLevelToBrushConverter.cs`
- Create: `02/ProductionLineMonitor.App/Converters/OperatingStateToBrushConverter.cs`
- Create: `02/ProductionLineMonitor.App/Themes/Colors.xaml`
- Create: `02/ProductionLineMonitor.App/Themes/Controls.xaml`
- Modify: `02/ProductionLineMonitor.App/App.xaml`

- [ ] **Step 1: Add immutable presentation types**

Use these public shapes:

```csharp
public enum OperatingState { Stopped, Running, Stopping }

public sealed record MetricCardItem(
    string Name,
    string Value,
    string Unit,
    string StatusText,
    MetricLevel Level);

public sealed record DashboardState(
    string DeviceName,
    string OnlineStatus,
    OperatingState OperatingState,
    string OperatingStatus,
    bool CanStart,
    bool CanStop,
    bool CanReset,
    IReadOnlyList<MetricCardItem> Metrics,
    IReadOnlyList<DeviceReading> RecentReadings,
    string LastUpdatedText,
    string Message);
```

These types intentionally do not implement `INotifyPropertyChanged`; the window replaces the whole DataContext snapshot once per sample.

- [ ] **Step 2: Add converter implementations**

`MetricLevelToBrushConverter` maps `Normal`, `Warning`, and `Alarm` to application resources `NormalBrush`, `WarningBrush`, and `AlarmBrush`. `OperatingStateToBrushConverter` maps `Stopped`, `Running`, and `Stopping` to `InactiveBrush`, `NormalBrush`, and `WarningBrush`. Invalid values return `InactiveBrush`. `ConvertBack` throws `NotSupportedException`.

- [ ] **Step 3: Add exact theme resource keys**

`Colors.xaml` must define:

```text
WindowBackgroundBrush, SurfaceBrush, SurfaceRaisedBrush, BorderBrush,
TextPrimaryBrush, TextSecondaryBrush, AccentBrush, AccentHoverBrush,
NormalBrush, WarningBrush, AlarmBrush, InactiveBrush
```

Use high-contrast dark neutral colors with green/amber/red status accents. `Controls.xaml` must define:

```text
PrimaryButtonStyle, StopButtonStyle, ResetButtonStyle,
StatusBadgeStyle, MetricCardBorderStyle, MonitorDataGridStyle,
MetricCardTemplate
```

The card template displays name, formatted value, unit, and status text; both the value and status badge use `MetricLevelToBrushConverter`.

- [ ] **Step 4: Merge resources in App.xaml**

Register both dictionaries and both converters under keys `MetricLevelToBrushConverter` and `OperatingStateToBrushConverter`. Keep `StartupUri="MainWindow.xaml"`.

- [ ] **Step 5: Build to catch XAML/resource errors**

```powershell
dotnet build .\02\ProductionLineMonitor.App\ProductionLineMonitor.App.csproj -c Release -warnaserror
```

Expected: `0 Warning(s)`, `0 Error(s)`. This build is the automated verification for resource/XAML wiring; visual behavior is covered in Task 8 because no UI automation dependency is introduced.

- [ ] **Step 6: Commit presentation resources**

```powershell
git add -- 02/ProductionLineMonitor.App/Models 02/ProductionLineMonitor.App/Converters 02/ProductionLineMonitor.App/Themes 02/ProductionLineMonitor.App/App.xaml
git commit -m "feat: add monitor presentation resources"
```

## Task 7: Implement the responsive window and lifecycle orchestration

**Files:**
- Modify: `02/ProductionLineMonitor.App/MainWindow.xaml`
- Modify: `02/ProductionLineMonitor.App/MainWindow.xaml.cs`
- Create: `02/README.md`

- [ ] **Step 1: Build the responsive XAML layout**

Set `Title="生产线设备监控 V1"`, initial `Width="1280"`, `Height="720"`, `MinWidth="1100"`, `MinHeight="640"`, `UseLayoutRounding="True"`, and `SnapsToDevicePixels="True"`.

Use a root `Grid` with rows `Auto, Auto, *, Auto`, maximum content width 1600, and 24 DIP outer margin. The required bindings are:

```text
DeviceName, OnlineStatus, OperatingStatus, OperatingState,
CanStart, CanStop, CanReset, Metrics, RecentReadings,
LastUpdatedText, Message
```

Use:

- header `Border` containing device/status `StackPanel` and three Click buttons;
- `ItemsControl ItemsSource="{Binding Metrics}"` with four-column `UniformGrid` and `MetricCardTemplate`;
- `DataGrid ItemsSource="{Binding RecentReadings}"` with explicit timestamp, temperature, pressure, speed, production, and overall status columns;
- footer status bar with last update, one-second sampling text, and message.

The DataGrid owns its scrolling and has row/column virtualization enabled. Do not wrap it in another `ScrollViewer`.

- [ ] **Step 2: Add window fields and initial DataContext**

Define fields for `FakeDataGenerator`, `AcquisitionService`, `RecentReadingBuffer`, latest reading, `OperatingState`, CTS, acquisition task, error/message text, and closing flags. Constructor calls `InitializeComponent()` and `RefreshDashboard("等待启动")`.

`RefreshDashboard` must call `Dispatcher.VerifyAccess()` and set a new `DashboardState` on `DataContext`. It creates four `MetricCardItem` values; before the first reading they show `--`, except production shows `0`.

- [ ] **Step 3: Implement Start, Stop, and Reset Click handlers**

Required method shapes:

```csharp
private async void StartButton_Click(object sender, RoutedEventArgs e);
private async void StopButton_Click(object sender, RoutedEventArgs e);
private void ResetButton_Click(object sender, RoutedEventArgs e);
private async Task RunAcquisitionAsync(CancellationTokenSource runCts);
private async Task StopAcquisitionAsync();
private Task ApplyReadingAsync(DeviceReading reading);
private void RefreshDashboard(string message);
```

Start guards against non-stopped state, creates a fresh CTS, sets `Running`, creates and stores the acquisition task, and awaits it. `RunAcquisitionAsync` treats cancellation as normal, catches other exceptions into a user-facing message, and in `finally` disposes only its own CTS, clears matching fields, and returns to stopped unless closing.

Stop sets `Stopping`, refreshes the DataContext, cancels the current CTS, and awaits the stored task. Reset is ignored unless stopped; it resets generator and buffer, clears latest reading, and refreshes.

`ApplyReadingAsync` performs exactly one `Dispatcher.InvokeAsync`; inside the UI delegate it calls `VerifyAccess`, skips updates while closing, adds the reading to the buffer, replaces the latest reading, and refreshes the DataContext.

- [ ] **Step 4: Implement the async closing handshake**

Wire `Closing="Window_Closing"`. The handler must use this sequence:

```csharp
private async void Window_Closing(object? sender, CancelEventArgs e)
{
    if (_shutdownCompleted) return;
    e.Cancel = true;
    _isClosing = true;
    _operatingState = OperatingState.Stopping;
    RefreshDashboard("正在停止采集并关闭窗口...");
    await StopAcquisitionAsync();
    _shutdownCompleted = true;
    Close();
}
```

No `.Wait()`, `.Result`, `Thread.Abort`, or direct background control changes are allowed.

- [ ] **Step 5: Write the beginner README**

Document:

- solution tree and project responsibilities;
- Java-to-C# analogies for `Task`, `CancellationToken`, event handlers, DataContext, and Dispatcher;
- build, test, and run commands;
- button behavior and status thresholds;
- why V1 uses binding snapshots instead of MVVM notification interfaces;
- UI-thread safety and closing sequence;
- 1280×720 and 1920×1080 acceptance checklist;
- links to both screenshots;
- V2 refactoring direction.

- [ ] **Step 6: Run complete automated verification**

```powershell
dotnet test .\02\ProductionLineMonitor.slnx -c Release
dotnet build .\02\ProductionLineMonitor.slnx -c Release --no-restore -warnaserror
git diff --check -- 02
```

Expected: tests have `0` failures, build has `0 Warning(s)` and `0 Error(s)`, diff check emits no errors.

- [ ] **Step 7: Commit the working V1 code**

```powershell
git add -- 02/ProductionLineMonitor.App/MainWindow.xaml 02/ProductionLineMonitor.App/MainWindow.xaml.cs 02/README.md
git commit -m "feat: complete WPF monitoring V1"
```

## Task 8: Perform UI acceptance, capture screenshots, and create V1 tag

**Files:**
- Create: `02/docs/screenshots/V1-1280x720.png`
- Create: `02/docs/screenshots/V1-1920x1080.png`
- Modify: `02/README.md` only if actual screenshot metadata or limitations differ.

- [ ] **Step 1: Start the Release build and observe 30 seconds**

```powershell
dotnet run --project .\02\ProductionLineMonitor.App\ProductionLineMonitor.App.csproj -c Release --no-build
```

Verify: Start begins one update per second; Stop freezes readings; Reset while stopped clears values and history; repeated clicks do not create duplicate loops; window movement and resize stay responsive.

- [ ] **Step 2: Verify both display targets**

At Windows display resolutions 1280×720 and 1920×1080, maximize the app and record the display scaling percentage. Verify no overlap, all buttons remain reachable, four cards remain readable, and all 20 rows are accessible through the DataGrid scrollbar.

- [ ] **Step 3: Capture and inspect screenshots**

Save exact files:

```text
02/docs/screenshots/V1-1280x720.png
02/docs/screenshots/V1-1920x1080.png
```

Each screenshot must show device name, online/running state, four metrics, three buttons, status colors, and sample rows. Inspect for personal notifications, account names, unrelated windows, or clipped content before committing.

- [ ] **Step 4: Verify closing stops the process**

Close the window while running, wait up to two seconds, then run:

```powershell
Get-Process ProductionLineMonitor.App -ErrorAction SilentlyContinue
```

Expected: no output. If a process remains, do not create the tag; diagnose the lifecycle failure first.

- [ ] **Step 5: Run final fresh verification**

```powershell
dotnet test .\02\ProductionLineMonitor.slnx -c Release
dotnet build .\02\ProductionLineMonitor.slnx -c Release --no-restore -warnaserror
git diff --check -- 02
git status --short
git tag --list V1
```

Expected: all tests pass, build has no warnings/errors, diff check is empty, only intended screenshot/README files are uncommitted, and no existing `V1` tag is listed.

- [ ] **Step 6: Commit screenshots without staging week 1**

```powershell
git add -- 02/docs/screenshots/V1-1280x720.png 02/docs/screenshots/V1-1920x1080.png 02/README.md
git diff --cached --check
git commit -m "docs: add WPF V1 acceptance screenshots"
```

- [ ] **Step 7: Create and verify the annotated tag**

```powershell
git tag -a V1 -m "V1: WPF fake-data monitoring interface"
git show --no-patch --decorate V1
git status --short --branch
```

Expected: `V1` points to the screenshot commit. Existing dirty files under `01` may remain, but no `01` file appears in any V1 commit. Do not push unless the user separately requests it.

## Plan self-review result

- Every requirement in the approved design maps to Tasks 2–8.
- All business behavior has a RED/GREEN test sequence before production implementation.
- UI-only XAML/resource behavior is verified by compilation and explicit manual acceptance without adding a UI automation dependency.
- Type names and method signatures are consistent across Core, App, and Tests.
- The implementation remains within V1: no ViewModel, `ICommand`, DI container, protocol, database, or remote push.
