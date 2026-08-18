namespace MvvmMonitor.Core.Models;

/// <summary>
/// 采集运行状态枚举。
/// </summary>
public enum OperatingState
{
    Stopped,
    Running,
    Stopping
}

/// <summary>
/// 将采集运行状态和完成原因转换为界面可直接展示的文本。
/// </summary>
public static class AcquisitionUiState
{
    /// <summary>
    /// 将运行状态转换为带图标的显示文本。
    /// </summary>
    public static string GetOperatingStatus(OperatingState state) => state switch
    {
        OperatingState.Stopped => "■ 已停止",
        OperatingState.Running => "▶ 运行中",
        OperatingState.Stopping => "⏸ 正在停止",
        _ => "未知"
    };

    /// <summary>
    /// 根据异常信息生成完成消息。
    /// </summary>
    public static string GetCompletionMessage(Exception? failure) =>
        failure is null ? "已停止" : $"采集异常: {failure.Message}";
}
