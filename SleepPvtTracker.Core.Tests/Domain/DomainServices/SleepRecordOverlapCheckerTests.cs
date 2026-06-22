using SleepPvtTracker.Core.Domain.DomainServices;
using SleepPvtTracker.Core.Interfaces;
using Moq;
using FluentAssertions;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.Domain.ValueObjects;

namespace SleepPvtTracker.Tests.Domain.DomainServices;

public class SleepRecordOverlapCheckerTest
{
    private readonly Mock<ISleepRecordRepository> _mockRepository;
    private readonly SleepRecordOverlapChecker _overlapChecker;

    public SleepRecordOverlapCheckerTest()
    {
        _mockRepository = new Mock<ISleepRecordRepository>();
        _overlapChecker = new SleepRecordOverlapChecker(_mockRepository.Object);
    }

    [Fact]
    public async Task CheckAsync_既存のレコードが0件の場合_Okを返すこと()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var period = SleepPeriod.Create(bedtime, wakeUpTime).Value;
        var newRecord = CreateValidSleepRecord(period);
        _mockRepository.Setup(r => r.GetAllRecordsAsync()).ReturnsAsync(new List<SleepRecord>());

        var result = await _overlapChecker.CheckAsync(newRecord);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CheckAsync_既存のレコードが重複しない場合_Okを返すこと()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var period = SleepPeriod.Create(bedtime, wakeUpTime).Value;
        var newRecord = CreateValidSleepRecord(period);

        //既存レコード
        var existsBedtime = new DateTime(2026, 6, 12, 17, 0, 0);
        var existsWakeUpTime = new DateTime(2026, 6, 12, 23, 0, 0);
        var existsPeriod = SleepPeriod.Create(existsBedtime, existsWakeUpTime).Value;
        var existsRecord = CreateValidSleepRecord(existsPeriod);

        _mockRepository.Setup(r => r.GetAllRecordsAsync()).ReturnsAsync(new List<SleepRecord> { existsRecord });

        var result = await _overlapChecker.CheckAsync(newRecord);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CheckAsync_既存のレコードが重複する場合_Failを返すこと()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var period = SleepPeriod.Create(bedtime, wakeUpTime).Value;
        var newRecord = CreateValidSleepRecord(period);

        //既存レコード
        var existingBedtime = new DateTime(2026, 6, 13, 6, 59, 0);
        var existingWakeUpTime = new DateTime(2026, 6, 13, 9, 0, 0);
        var existingPeriod = SleepPeriod.Create(existingBedtime, existingWakeUpTime).Value;
        var existingRecord = CreateValidSleepRecord(existingPeriod);

        _mockRepository.Setup(r => r.GetAllRecordsAsync()).ReturnsAsync(new List<SleepRecord> { existingRecord });

        var result = await _overlapChecker.CheckAsync(newRecord);

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("重複");
    }

    private static SleepRecord CreateValidSleepRecord(SleepPeriod sleepPeriod)
    {
        var currentTime = sleepPeriod.WakeUpTime.AddHours(1);
        return SleepRecord.Create(sleepPeriod, SubjectiveSleepiness.Create(5).Value, currentTime).Value;
    }
}