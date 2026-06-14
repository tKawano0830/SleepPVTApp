using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentAssertions;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.Exceptions;
using SleepPvtTracker.Core.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.Tests.Entities;

public class SleepRecordTests
{
    [Fact]
    public void Create_正常な値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        var record = SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness, currentTime);

        record.Should().NotBeNull();
        record.Id.Value.Should().NotBeEmpty();
        record.Duration.TotalHours.Should().Be(8);
        record.TargetDate.Should().Be(new DateOnly(2026, 6, 13));
        record.Comments.Should().BeEmpty();
    }

    [Fact]
    public void Create_就寝時間が起床時間より未来の場合_例外をスローすること()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 12, 7, 0, 0);

        Action act = () => SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness, currentTime);

        act.Should().Throw<DomainException>().WithMessage("*起床時間*");
    }

    [Fact]
    public void Create_起床時間が現在時刻より未来の場合_例外をスローすること()
    {
        var currentTime = new DateTime(2026, 6, 12, 6, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 12, 7, 0, 0);

        Action act = () => SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness, currentTime);

        act.Should().Throw<DomainException>().WithMessage("*起床時間*");
    }

    [Fact]
    public void Create_睡眠時間が24時間以上の場合_例外をスローすること()
    {
        var currentTime = new DateTime(2026, 6, 14, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 23, 0, 0);

        Action act = () => SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness, currentTime);

        act.Should().Throw<DomainException>().WithMessage("*睡眠時間*");
    }

    [Fact]
    public void UpdateComments_正常な値を渡した場合_正しいプロパティがセットされていること()
    {
        var record = CreateValidRecord();
        var validComments = new string('a', SleepRecord.MaxCommentsLength);

        record.UpdateComments(validComments);

        record.Comments.Should().Be(validComments);
    }

    [Fact]
    public void UpdateComments_既定の文字数を超える場合_例外をスローすること()
    {
        var invalidComments = new string('a', SleepRecord.MaxCommentsLength + 1);

        Action act = () => CreateValidRecord().UpdateComments(invalidComments);

        act.Should().Throw<DomainException>().WithMessage("*コメント*");
    }

    [Fact]
    public void RecordPvt_開始時間が既定時間ちょうどの場合_正しいプロパティがセットされること()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var record = SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness, currentTime);

        var validPvtStart = wakeUpTime.AddMinutes(SleepRecord.PvtAvailableMinutes);
        var pvtResult = PvtResult.Create(validPvtStart, validPvtStart.AddMinutes(3), new List<PvtTrial>(), 0);

        record.RecordPvt(pvtResult);

        record.PvtResult.Should().BeEquivalentTo(pvtResult);
    }

    [Fact]
    public void RecordPvt_開始時間が既定時間を1秒でも超過した場合_例外をスローすること()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var record = SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness, currentTime);

        var invalidPvtStart = wakeUpTime.AddMinutes(SleepRecord.PvtAvailableMinutes).AddSeconds(1);
        var pvtResult = PvtResult.Create(invalidPvtStart, invalidPvtStart.AddMinutes(3), new List<PvtTrial>(), 0);

        Action act = () => record.RecordPvt(pvtResult);

        act.Should().Throw<DomainException>();
    }

    private static SleepRecord CreateValidRecord()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        return SleepRecord.Create(bedtime, wakeUpTime, ValidSleepness, DateTime.Now);
    }
    private static readonly SubjectiveSleepiness ValidSleepness = SubjectiveSleepiness.Create(5);

}