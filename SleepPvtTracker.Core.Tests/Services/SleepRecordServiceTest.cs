using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SleepPvtTracker.Core.DTOs;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.Exceptions;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Core.Services;
using SleepPvtTracker.Core.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.Tests.Services;

public class SleepRecordServiceTests
{
    private readonly Mock<ISleepRecordRepository> _mockReoisitory;
    private readonly SleepRecordService _service;
    private static readonly Guid DummyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public SleepRecordServiceTests()
    {
        _mockReoisitory = new Mock<ISleepRecordRepository>();
        _service = new SleepRecordService(_mockReoisitory.Object);
    }

    [Fact]
    public async Task CreateSleepRecordAsync_正しいDTOが渡された場合_AddAsyncが1回呼ばれること()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var dto = new CreateSleepRecordDto
        {
            Bedtime = bedtime,
            WakeUpTime = wakeUpTime,
            SleepinessLevel = 1,
            Comments = "テスト"
        };

        await _service.CreateSleepRecordAsync(dto);

        _mockReoisitory.Verify(r => r.AddAsync(It.IsAny<SleepRecord>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSleepRecordAsync_削除対象の睡眠記録が存在する場合_DeleteAsyncが1回呼ばれること()
    {
        var sleepRecordId = DummyId;
        var dummyRecord = CreateDummySleepRecord();

        _mockReoisitory.Setup(r => r.GetByIdAsync(sleepRecordId)).ReturnsAsync(dummyRecord);

        await _service.DeleteSleepRecordAsync(sleepRecordId);

        _mockReoisitory.Verify(r => r.DeleteAsync(It.IsAny<SleepRecord>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSleepRecordAsync_削除対象の睡眠記録が存在しない場合_例外がスローされること()
    {
        var sleepRecordId = DummyId;

        _mockReoisitory.Setup(r => r.GetByIdAsync(sleepRecordId)).ReturnsAsync((SleepRecord?)null);

        Func<Task> act = async () => await _service.DeleteSleepRecordAsync(sleepRecordId);

        await act.Should().ThrowAsync<EntityNotFoundException>().WithMessage("*睡眠記録*");

        _mockReoisitory.Verify(repo => repo.DeleteAsync(It.IsAny<SleepRecord>()), Times.Never);
    }

    [Fact]
    public async Task SubmitPvtAsync_更新対象の睡眠記録が存在する場合_UpdateAsyncが1回呼ばれること()
    {
        var sleepRecordId = DummyId;
        var dummyRecord = CreateDummySleepRecord();
        var dto = new SubmitPvtDto
        {
            SleepRecordId = sleepRecordId,
            StartTime = dummyRecord.WakeUpTime.AddMinutes(10),
            EndTime = dummyRecord.WakeUpTime.AddMinutes(13),
            Trials = new List<PvtTrialDto> { new PvtTrialDto { ChangedAt = 1000, ClickedAt = 1250 } }
        };

        _mockReoisitory.Setup(r => r.GetByIdAsync(sleepRecordId)).ReturnsAsync(dummyRecord);

        await _service.SubmitPvtAsync(dto);

        _mockReoisitory.Verify(r => r.UpdateAsync(It.IsAny<SleepRecord>()), Times.Once);
    }

    [Fact]
    public async Task SubmitPvtAsync_更新対象の睡眠記録が存在しない場合_例外がスローされること()
    {
        var sleepRecordId = DummyId;
        var dummyRecord = CreateDummySleepRecord();
        var dto = new SubmitPvtDto
        {
            SleepRecordId = sleepRecordId,
            StartTime = dummyRecord.WakeUpTime.AddMinutes(10),
            EndTime = dummyRecord.WakeUpTime.AddMinutes(13),
            Trials = new List<PvtTrialDto> { new PvtTrialDto { ChangedAt = 1000, ClickedAt = 1250 } }
        };

        _mockReoisitory.Setup(r => r.GetByIdAsync(sleepRecordId)).ReturnsAsync((SleepRecord?)null);

        Func<Task> act = async () => await _service.SubmitPvtAsync(dto);

        await act.Should().ThrowAsync<EntityNotFoundException>().WithMessage("*睡眠記録*");

        _mockReoisitory.Verify(repo => repo.UpdateAsync(It.IsAny<SleepRecord>()), Times.Never);
    }

    [Fact]
    public async Task GetAllSleepRecordAsync_睡眠記録データが1件以上存在する場合_レコードリストを返すこと()
    {
        var expectedRecords = new List<SleepRecord>
        {
            CreateDummySleepRecord(),
            CreateDummySleepRecord()
        };
        _mockReoisitory.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedRecords);

        var result = await _service.GetAllSleepRecordsAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedRecords);

        _mockReoisitory.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllSleepRecordAsync_睡眠記録データが存在しない場合_空のリストを返すこと()
    {
        _mockReoisitory.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SleepRecord>());

        var result = await _service.GetAllSleepRecordsAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockReoisitory.Verify(r => r.GetAllAsync(), Times.Once);
    }

    private static SleepRecord CreateDummySleepRecord()
    {
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);

        return SleepRecord.Create(bedtime, wakeUpTime, SubjectiveSleepiness.Create(5));
    }
}