using DataPersistence.Core.Entities;
using DataPersistence.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataPersistence.Data.Repositories;

/// <summary>
/// 机床状态仓储实现 —— 使用 EF Core 操作 SQLite。
/// </summary>
public class MachineStateRepository : IMachineStateRepository
{
    private readonly MonitoringDbContext _db;

    public MachineStateRepository(MonitoringDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task AddAsync(MachineStateRecord record, CancellationToken cancellationToken = default)
    {
        _db.MachineStates.Add(record);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MachineStateRecord>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        // 0 条不是“至少返回 1 条”的同义词；显式拒绝可尽早暴露调用方分页错误。
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);

        return await _db.MachineStates
            .OrderByDescending(r => r.Timestamp)
            .Take(Math.Min(count, 10000))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MachineStateRecord>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await _db.MachineStates
            .Where(r => r.Timestamp >= start && r.Timestamp <= end)
            .OrderByDescending(r => r.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.MachineStates.CountAsync(cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _db.MachineStates.ExecuteDeleteAsync(cancellationToken);
    }
}
