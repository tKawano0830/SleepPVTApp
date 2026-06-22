using System;
using FluentAssertions;
using SleepPvtTracker.Core.Domain.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.Tests.Domain.ValueObjects;

//※MainSleepは完全に撤廃しましたがドキュメントとして残します。
public class MainSleepTests
{

    public void Create_正常な値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        var mainSleep = MainSleep.Create(bedtime, wakeUpTime);

        mainSleep.Should().NotBeNull();
        mainSleep.Bedtime.Should().Be(bedtime);
        mainSleep.WakeUpTime.Should().Be(wakeUpTime);
        mainSleep.Duration.TotalHours.Should().Be(8);
    }


    public void Create_就寝時間が起床時間より未来の場合_例外をスローすること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 12, 7, 0, 0);

        Action act = () => MainSleep.Create(bedtime, wakeUpTime);

        act.Should().Throw<ArgumentException>().WithMessage("*起床時間*");
    }


    public void Create_睡眠時間が24時間以上の場合_例外をスローすること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 23, 0, 0);

        Action act = () => MainSleep.Create(bedtime, wakeUpTime);

        act.Should().Throw<ArgumentException>().WithMessage("*睡眠時間*");
    }
}