using FluentAssertions;
using SleepPvtTracker.Core.Domain.ValueObjects;

namespace SleepPvtTracker.Tests.Domain.ValueObjects;

public class SleepPeriodTest
{
    [Fact]
    public void Create_正常な値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        var period = SleepPeriod.Create(bedtime, wakeUpTime);

        period.IsSuccess.Should().BeTrue();
        period.Value.Should().NotBeNull();
        period.Value.Duration.TotalHours.Should().Be(8);
        period.Value.Bedtime.Should().Be(bedtime);
        period.Value.WakeUpTime.Should().Be(wakeUpTime);
    }

    [Fact]
    public void Create_就寝時間が起床時間より未来の場合_失敗のResultを返すこと()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 12, 7, 0, 0);

        var period = SleepPeriod.Create(bedtime, wakeUpTime);

        period.IsFailure.Should().BeTrue();
        period.ErrorMessage.Should().Contain("起床時間");
    }

    [Fact]
    public void Create_睡眠時間が24時間以上の場合_失敗のResultを返すこと()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 23, 0, 0);

        var period = SleepPeriod.Create(bedtime, wakeUpTime);

        period.IsFailure.Should().BeTrue();
        period.ErrorMessage.Should().Contain("睡眠時間");
    }
}