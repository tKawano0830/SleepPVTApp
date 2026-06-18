using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualBasic;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.ValueObjects;

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

        var record1Id = new SleepRecordId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var record2Id = new SleepRecordId(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        var baseDate = new DateTime(2026, 6, 13, 7, 0, 0);

        modelBuilder.Entity<SleepRecord>().HasData(
            new
            {
                Id = record1Id,
                Bedtime = baseDate.AddDays(-2).AddHours(-8),
                WakeUpTime = baseDate.AddDays(-2),
                Comments = "よく眠れた"
            },
            new
            {
                Id = record2Id,
                Bedtime = baseDate.AddDays(-1).AddHours(-6),
                WakeUpTime = baseDate.AddDays(-1),
                Comments = string.Empty
            }
        );

        modelBuilder.Entity<SleepRecord>().OwnsOne(e => e.Sleepiness).HasData(
            new { SleepRecordId = record1Id, Level = 1 },
            new { SleepRecordId = record2Id, Level = 7 }
        );
    }

}
