using DataPersistence.Core.Entities;
using DataPersistence.Data;
using DataPersistence.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataPersistence.Tests.Repositories;

public sealed class MachineStateRepositoryTests : IDisposable
{
    private readonly MonitoringDbContext _db;
    private readonly MachineStateRepository _repo;

    public MachineStateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MonitoringDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        _db = new MonitoringDbContext(options);
        _db.Database.OpenConnection();
        _db.Database.EnsureCreated();
        _repo = new MachineStateRepository(_db);
    }

    [Fact]
    public async Task AddAsync_SavesRecord()
    {
        var record = new MachineStateRecord { SpindleSpeed = 1500, FeedSpeed = 100 };
        await _repo.AddAsync(record);
        Assert.True(record.Id > 0);
    }

    [Fact]
    public async Task AddAsync_AfterContextIsReopened_RecordStillExists()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"machine-state-{Guid.NewGuid():N}.db");
        var options = new DbContextOptionsBuilder<MonitoringDbContext>()
            .UseSqlite($"Data Source={databasePath};Pooling=False")
            .Options;

        try
        {
            await using (var writeDb = new MonitoringDbContext(options))
            {
                await writeDb.Database.EnsureCreatedAsync();
                var writeRepository = new MachineStateRepository(writeDb);
                await writeRepository.AddAsync(new MachineStateRecord
                {
                    CurrentProgramName = "LEARNING-001",
                    SpindleSpeed = 1500
                });
            }

            // 新 DbContext 会建立新连接；从这里读到数据才证明 SQLite 文件真正持久化。
            await using var readDb = new MonitoringDbContext(options);
            var saved = await readDb.MachineStates.SingleAsync();
            Assert.Equal("LEARNING-001", saved.CurrentProgramName);
            Assert.Equal(1500, saved.SpindleSpeed);
        }
        finally
        {
            File.Delete(databasePath);
        }
    }

    [Fact]
    public async Task GetLatestAsync_ReturnsNewestFirst()
    {
        var baseline = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        await _repo.AddAsync(new MachineStateRecord { SpindleSpeed = 3000, Timestamp = baseline.AddSeconds(1) });
        await _repo.AddAsync(new MachineStateRecord { SpindleSpeed = 1000, Timestamp = baseline.AddSeconds(3) });
        await _repo.AddAsync(new MachineStateRecord { SpindleSpeed = 2000, Timestamp = baseline.AddSeconds(2) });

        var result = await _repo.GetLatestAsync(3);
        Assert.Equal(3, result.Count);
        Assert.Equal(
            [baseline.AddSeconds(3), baseline.AddSeconds(2), baseline.AddSeconds(1)],
            result.Select(item => item.Timestamp));
    }

    [Fact]
    public async Task GetLatestAsync_WhenCountIsNotPositive_Throws()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _repo.GetLatestAsync(0));
    }

    [Fact]
    public async Task GetByTimeRangeAsync_ReturnsOnlyInclusiveRangeNewestFirst()
    {
        var baseline = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var start = baseline.AddHours(-1);
        var end = baseline.AddHours(1);
        await _repo.AddAsync(new MachineStateRecord { Timestamp = baseline.AddHours(-2) });
        await _repo.AddAsync(new MachineStateRecord { Timestamp = start });
        await _repo.AddAsync(new MachineStateRecord { Timestamp = baseline.AddMinutes(-30) });
        await _repo.AddAsync(new MachineStateRecord { Timestamp = baseline.AddMinutes(30) });
        await _repo.AddAsync(new MachineStateRecord { Timestamp = end });
        await _repo.AddAsync(new MachineStateRecord { Timestamp = baseline.AddHours(2) });

        var result = await _repo.GetByTimeRangeAsync(start, end);

        Assert.Equal(
            [end, baseline.AddMinutes(30), baseline.AddMinutes(-30), start],
            result.Select(item => item.Timestamp));
    }

    [Fact]
    public async Task GetCountAsync_ReturnsCorrectCount()
    {
        for (int i = 0; i < 10; i++)
            await _repo.AddAsync(new MachineStateRecord());

        var count = await _repo.GetCountAsync();
        Assert.Equal(10, count);
    }

    [Fact]
    public async Task ClearAsync_RemovesAllRecords()
    {
        for (int i = 0; i < 5; i++)
            await _repo.AddAsync(new MachineStateRecord());

        await _repo.ClearAsync();
        var count = await _repo.GetCountAsync();
        Assert.Equal(0, count);
    }

    public void Dispose()
    {
        _db.Database.CloseConnection();
        _db.Dispose();
    }
}
