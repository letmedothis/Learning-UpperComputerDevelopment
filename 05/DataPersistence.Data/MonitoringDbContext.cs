using DataPersistence.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataPersistence.Data;

/// <summary>
/// 机床监控数据库上下文。
/// </summary>
public class MonitoringDbContext : DbContext
{
    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
        : base(options)
    {
    }

    public DbSet<MachineStateRecord> MachineStates => Set<MachineStateRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MachineStateRecord>(entity =>
        {
            entity.ToTable("MachineStates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Timestamp);
        });
    }
}
