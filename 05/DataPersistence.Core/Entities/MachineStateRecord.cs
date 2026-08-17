namespace DataPersistence.Core.Entities;

/// <summary>
/// 机床状态记录 —— 持久化到数据库的实体。
/// </summary>
public class MachineStateRecord
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public int ConnectionStatus { get; set; }
    public int RunStatus { get; set; }
    public long WorkpieceCount { get; set; }
    public string? CurrentProgramName { get; set; }
    public double SpindleSpeed { get; set; }
    public double SpindleLoad { get; set; }
    public double FeedSpeed { get; set; }
    public string? CurrentAlarm { get; set; }
}
