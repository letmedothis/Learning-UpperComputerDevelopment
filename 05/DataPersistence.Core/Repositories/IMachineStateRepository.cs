using DataPersistence.Core.Entities;

namespace DataPersistence.Core.Repositories;

/// <summary>
/// 机床状态仓储接口 —— 定义数据访问契约。
/// </summary>
public interface IMachineStateRepository
{
    /// <summary>添加一条状态记录。</summary>
    Task AddAsync(MachineStateRecord record, CancellationToken cancellationToken = default);

    /// <summary>获取最新的 N 条记录，按时间倒序。</summary>
    Task<IReadOnlyList<MachineStateRecord>> GetLatestAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>按时间范围查询记录。</summary>
    Task<IReadOnlyList<MachineStateRecord>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    /// <summary>获取记录总数。</summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>清空所有记录。</summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
