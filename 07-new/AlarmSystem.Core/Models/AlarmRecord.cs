namespace AlarmSystem.Core.Models;

/// <summary>
/// 报警记录。
/// </summary>
public sealed record AlarmRecord(
    string Id,
    string RuleName,
    string MetricName,
    double Value,
    double Threshold,
    AlarmLevel Level,
    string Message,
    DateTime Timestamp,
    bool IsAcknowledged = false);
