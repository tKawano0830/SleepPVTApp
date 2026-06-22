using FluentAssertions;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.Domain.ValueObjects;

namespace SleepPvtTracker.Core.Tests.Domain.Entities;

public class SleepRecordTests
{
    [Fact]
    public void Create_正常な値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var validSleepPeriod = SleepPeriod.Create(bedtime, wakeUpTime).Value;

        var record = SleepRecord.Create(validSleepPeriod, ValidSleepness, currentTime);

        record.IsSuccess.Should().BeTrue();
        record.Value.Should().NotBeNull();
        record.Value.Id.Value.Should().NotBeEmpty();
        record.Value.TargetDate.Should().Be(new DateOnly(2026, 6, 13));
        record.Value.Comments.Should().BeEmpty();
    }

    [Fact]
    public void Create_起床時間が現在時刻より未来の場合_失敗のResultを返すこと()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var validSleepPeriod = SleepPeriod.Create(bedtime, wakeUpTime).Value;

        var currentTime = new DateTime(2026, 6, 12, 6, 0, 0);
        var record = SleepRecord.Create(validSleepPeriod, ValidSleepness, currentTime);

        record.IsFailure.Should().BeTrue();
        record.ErrorMessage.Should().Contain("起床時間");
    }

    [Fact]
    public void UpdateComments_正常な値を渡した場合_正しいプロパティがセットされていること()
    {
        var record = CreateValidRecord();
        var validComments = new string('a', SleepRecord.MaxCommentsLength);

        var result = record.UpdateComments(validComments);

        result.IsSuccess.Should().BeTrue();
        record.Comments.Should().Be(validComments);
    }

    [Fact]
    public void UpdateComments_既定の文字数を超える場合_失敗のResultを返すこと()
    {
        var record = CreateValidRecord();
        var invalidComments = new string('a', SleepRecord.MaxCommentsLength + 1);

        var result = record.UpdateComments(invalidComments);

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("コメント");
        record.Comments.Should().BeEmpty();
    }

    [Fact]
    public void RecordPvt_開始時間が既定時間ちょうどの場合_正しいプロパティがセットされること()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var validSleepPeriod = SleepPeriod.Create(bedtime, wakeUpTime).Value;
        var record = SleepRecord.Create(validSleepPeriod, ValidSleepness, currentTime).Value;

        var validPvtStart = wakeUpTime.AddMinutes(SleepRecord.PvtAvailableMinutes);
        var pvtResult = PvtResult.Create(validPvtStart, validPvtStart.AddMinutes(3), new List<PvtTrial>(), 0).Value;

        var result = record.RecordPvt(pvtResult);

        result.IsSuccess.Should().BeTrue();
        record.PvtResult.Should().BeEquivalentTo(pvtResult);
    }

    [Fact]
    public void RecordPvt_開始時間が既定時間を1秒でも超過した場合_失敗のResultを返すこと()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var validSleepPeriod = SleepPeriod.Create(bedtime, wakeUpTime).Value;
        var record = SleepRecord.Create(validSleepPeriod, ValidSleepness, currentTime).Value;

        var invalidPvtStart = wakeUpTime.AddMinutes(SleepRecord.PvtAvailableMinutes).AddSeconds(1);
        var pvtResult = PvtResult.Create(invalidPvtStart, invalidPvtStart.AddMinutes(3), new List<PvtTrial>(), 0).Value;

        var result = record.RecordPvt(pvtResult);

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("PVT");
        record.PvtResult.Should().BeNull();
    }

    private static SleepRecord CreateValidRecord()
    {
        return SleepRecord.Create(CreateValidSleepPeriod(), ValidSleepness, DateTime.Now).Value;
    }
    private static SleepPeriod CreateValidSleepPeriod()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        return SleepPeriod.Create(new DateTime(2026, 6, 12, 23, 0, 0), new DateTime(2026, 6, 13, 7, 0, 0)).Value;
    }
    private static readonly SubjectiveSleepiness ValidSleepness = SubjectiveSleepiness.Create(5).Value;
}