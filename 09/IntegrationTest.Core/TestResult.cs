namespace IntegrationTest.Core;

/// <summary>
/// 测试结果。
/// </summary>
public sealed class TestResult
{
    public int SetupStepsCompleted { get; set; }
    public int TestsPassed { get; set; }
    public int TestsFailed { get; set; }
    public int CleanupStepsCompleted { get; set; }
    public List<string> Errors { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public bool IsSuccess => TestsFailed == 0 && Errors.Count == 0;
    public int TotalTests => TestsPassed + TestsFailed;
}
