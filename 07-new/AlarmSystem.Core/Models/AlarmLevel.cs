namespace AlarmSystem.Core.Models;

/// <summary>
/// 报警级别，按严重程度升序排列。
/// </summary>
public enum AlarmLevel
{
    /// <summary>信息提示。</summary>
    Info = 0,

    /// <summary>警告，需关注。</summary>
    Warning = 1,

    /// <summary>报警，需处理。</summary>
    Alarm = 2,

    /// <summary>严重故障，需立即响应。</summary>
    Critical = 3
}
