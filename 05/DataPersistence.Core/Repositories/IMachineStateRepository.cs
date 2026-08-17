using DataPersistence.Core.Entities;

namespace DataPersistence.Core.Repositories;

/// <summary>
/// 机床状态仓储接口 —— 定义数据访问契约。
/// </summary>
public interface IMachineStateRepository
{
    Task AddAsync(MachineStateRecord record, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MachineStateRecord>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MachineStateRecord>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}
