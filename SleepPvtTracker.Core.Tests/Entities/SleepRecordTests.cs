using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentAssertions;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.Tests.Entities;

public class SleepRecordTests
{
    [Fact]
    public void Create_正常な値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        var record = SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness);

        record.Should().NotBeNull();
        record.Id.Should().NotBeEmpty();
        record.Duration.TotalHours.Should().Be(8);
        record.TargetDate.Should().Be(new DateOnly(2026, 6, 13));
        record.Comments.Should().BeEmpty();
    }

    [Fact]
    public void Create_就寝時間が起床時間より未来の場合_例外をスローすること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 12, 7, 0, 0);

        Action act = () => SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness);

        act.Should().Throw<ArgumentException>().WithMessage("*起床時間*");
    }

    [Fact]
    public void Create_睡眠時間が24時間以上の場合_例外をスローすること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 23, 0, 0);

        Action act = () => SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness);

        act.Should().Throw<ArgumentException>().WithMessage("*睡眠時間*");
    }

    [Fact]
    public void UpdateComments_正常な値を渡した場合_正しいプロパティがセットされていること()
    {
        var record = CreateValidRecord();
        var validComments = new string('a', 100);

        record.UpdateComments(validComments);

        record.Comments.Should().Be(validComments);
    }

    //ユースケース層に移動させたので、エンティティのルールから削除
    // public void UpdateComments_100文字を超える場合_例外をスローすること()
    // {
    //     var invalidComments = new string('a', 101);

    //     Action act = () => CreateValidRecord().UpdateComments(invalidComments);

    //     act.Should().Throw<ArgumentException>().WithMessage("*コメント*");
    // }

    [Fact]
    public void RecordPvt_開始時間が起床後90分以内の場合_例外をスローすること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var record = SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness);

        var invalidPvtStart = new DateTime(2026, 6, 13, 8, 30, 0);
        var pvtResult = PvtResult.Create(invalidPvtStart, invalidPvtStart.AddMinutes(3), new List<PvtTrial>(), 0);

        record.RecordPvt(pvtResult);

        record.PvtResult.Should().BeEquivalentTo(pvtResult);
    }

    [Fact]
    public void RecordPvt_開始時間が起床後90分超過の場合_例外をスローすること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var record = SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness);

        var invalidPvtStart = new DateTime(2026, 6, 13, 8, 30, 1);
        var pvtResult = PvtResult.Create(invalidPvtStart, invalidPvtStart.AddMinutes(3), new List<PvtTrial>(), 0);

        Action act = () => record.RecordPvt(pvtResult);

        act.Should().Throw<InvalidOperationException>();
    }

    private static SleepRecord CreateValidRecord()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        return SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness);
    }
    private static readonly SubjectiveSleepiness ValidSleepness = SubjectiveSleepiness.Create(5);

}