namespace IntegrationTest.Core;

/// <summary>
/// 测试结果。
/// </summary>
public sealed class TestResult
{
    /// <summary>已完成的初始化步骤数。</summary>
    public int SetupStepsCompleted { get; set; }

    /// <summary>通过的测试数。</summary>
    public int TestsPassed { get; set; }

    /// <summary>失败的测试数。</summary>
    public int TestsFailed { get; set; }

    /// <summary>已完成的清理步骤数。</summary>
    public int CleanupStepsCompleted { get; set; }

    /// <summary>错误信息列表。</summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>测试总耗时。</summary>
    public TimeSpan Duration { get; set; }

    /// <summary>是否全部通过（无失败且无错误）。</summary>
    public bool IsSuccess => TestsFailed == 0 && Errors.Count == 0;

    /// <summary>测试总数。</summary>
    public int TotalTests => TestsPassed + TestsFailed;
}
