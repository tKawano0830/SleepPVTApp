using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualBasic;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.Domain.ValueObjects;

namespace SleepPvtTracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<SleepRecord> SleepRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SleepRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
            .HasConversion(id => id.Value, value => new SleepRecordId(value))
            .ValueGeneratedNever();

            entity.OwnsOne(e => e.SleepPeriod, sleepPeriod =>
            {
                sleepPeriod.Property(p => p.Bedtime).HasColumnName("Bedtime");
                sleepPeriod.Property(p => p.WakeUpTime).HasColumnName("WakeUpTime");
            });

            entity.Property(e => e.Comments).HasMaxLength(100);

            entity.OwnsOne(e => e.Sleepiness, sleepiness =>
            {
                sleepiness.Property(s => s.Level).HasColumnName("SleepinessLevel");
            });

            entity.OwnsOne(e => e.PvtResult, pvt =>
            {
                pvt.Property(p => p.StartTime).HasColumnName("PvtStartTime");
                pvt.Property(p => p.EndTime).HasColumnName("PvtEndTime");
                pvt.Property(p => p.ExtraFalseStarts).HasColumnName("PvtExtraFalseStarts");

                //trialsはjson変換して同一テーブルに保存する
                pvt.Property(p => p.Trials)
                .HasColumnName("PvtTrialsJson")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<IReadOnlyList<PvtTrial>>(v, (JsonSerializerOptions)null!)!
                ).Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<PvtTrial>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));
            });
        });

    }

}
