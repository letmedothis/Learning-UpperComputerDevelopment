namespace IntegrationTest.Core;

/// <summary>
/// 集成测试运行器 —— 协调多个组件的测试。
/// </summary>
public sealed class TestRunner
{
    private readonly List<Func<Task>> _setupSteps = new();
    private readonly List<Func<Task<bool>>> _testSteps = new();
    private readonly List<Func<Task>> _cleanupSteps = new();

    /// <summary>
    /// 添加设置步骤。
    /// </summary>
    public TestRunner AddSetup(Func<Task> step)
    {
        _setupSteps.Add(step);
        return this;
    }

    /// <summary>
    /// 添加测试步骤。
    /// </summary>
    public TestRunner AddTest(Func<Task<bool>> step)
    {
        _testSteps.Add(step);
        return this;
    }

    /// <summary>
    /// 添加清理步骤。
    /// </summary>
    public TestRunner AddCleanup(Func<Task> step)
    {
        _cleanupSteps.Add(step);
        return this;
    }

    /// <summary>
    /// 运行所有测试。
    /// </summary>
    public async Task<TestResult> RunAsync()
    {
        var result = new TestResult();
        var startTime = DateTime.Now;

        try
        {
            // 执行设置
            foreach (var step in _setupSteps)
            {
                await step();
                result.SetupStepsCompleted++;
            }

            // 执行测试
            foreach (var step in _testSteps)
            {
                try
                {
                    var passed = await step();
                    if (passed)
                        result.TestsPassed++;
                    else
                        result.TestsFailed++;
                }
                catch (Exception ex)
                {
                    result.TestsFailed++;
                    result.Errors.Add(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Setup failed: {ex.Message}");
        }
        finally
        {
            // 执行清理
            foreach (var step in _cleanupSteps)
            {
                try
                {
                    await step();
                    result.CleanupStepsCompleted++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Cleanup failed: {ex.Message}");
                }
            }

            result.Duration = DateTime.Now - startTime;
        }

        return result;
    }
}
