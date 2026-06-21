using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SleepPvtTracker.Core.UseCases.Dtos;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Core.UseCases;
using SleepPvtTracker.Core.Domain.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.Tests.UseCases;

public class SleepRecordUseCaseTests
{
    private readonly Mock<ISleepRecordRepository> _mockReoisitory;
    private readonly SleepRecordUseCase _service;
    private static readonly SleepRecordId DummyId = new SleepRecordId(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    public SleepRecordUseCaseTests()
    {
        _mockReoisitory = new Mock<ISleepRecordRepository>();
        _service = new SleepRecordUseCase(_mockReoisitory.Object);
    }

    [Fact]
    public async Task CreateSleepRecordAsync_正しいDTOが渡された場合_AddAsyncが1回呼ばれること()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var dto = new CreateSleepRecordDto
        {
            Bedtime = bedtime,
            WakeUpTime = wakeUpTime,
            SleepinessLevel = 1,
            Comments = "テスト"
        };

        var result = await _service.CreateSleepRecordAsync(dto, currentTime);

        result.IsSuccess.Should().BeTrue();
        _mockReoisitory.Verify(r => r.AddRecordAsync(It.IsAny<SleepRecord>()), Times.Once);
    }

    [Fact]
    public async Task CreateSleepRecordAsync_ドメインルール違反のDTOが渡された場合_失敗のResultを返すこと()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var validWakeUpTime = new DateTime(2026, 6, 12, 7, 0, 0);//就寝時間より過去
        var dto = new CreateSleepRecordDto
        {
            Bedtime = bedtime,
            WakeUpTime = validWakeUpTime,
            SleepinessLevel = 1,
            Comments = "テスト"
        };

        var result = await _service.CreateSleepRecordAsync(dto, currentTime);

        result.IsFailure.Should().BeTrue();
        _mockReoisitory.Verify(r => r.AddRecordAsync(It.IsAny<SleepRecord>()), Times.Never);
    }

    [Fact]
    public async Task DeleteSleepRecordAsync_削除対象の睡眠記録が存在する場合_DeleteAsyncが1回呼ばれること()
    {
        var sleepRecordId = DummyId;
        var dummyRecord = CreateDummySleepRecord();

        _mockReoisitory.Setup(r => r.GetRecordByIdAsync(sleepRecordId)).ReturnsAsync(dummyRecord);

        var result = await _service.DeleteSleepRecordAsync(sleepRecordId.Value);

        result.IsSuccess.Should().BeTrue();
        _mockReoisitory.Verify(r => r.DeleteRecordAsync(It.IsAny<SleepRecord>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSleepRecordAsync_削除対象の睡眠記録が存在しない場合_失敗のResultを返すこと()
    {
        var sleepRecordId = DummyId;

        _mockReoisitory.Setup(r => r.GetRecordByIdAsync(sleepRecordId)).ReturnsAsync((SleepRecord?)null);

        var result = await _service.DeleteSleepRecordAsync(sleepRecordId.Value);

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("削除対象");
        _mockReoisitory.Verify(repo => repo.DeleteRecordAsync(It.IsAny<SleepRecord>()), Times.Never);
    }

    [Fact]
    public async Task SubmitPvtAsync_更新対象の睡眠記録が存在する場合_UpdateAsyncが1回呼ばれること()
    {
        var sleepRecordId = DummyId;
        var dummyRecord = CreateDummySleepRecord();
        var dto = new SubmitPvtDto
        {
            SleepRecordId = sleepRecordId.Value,
            StartTime = dummyRecord.SleepPeriod.WakeUpTime.AddMinutes(10),
            EndTime = dummyRecord.SleepPeriod.WakeUpTime.AddMinutes(13),
            Trials = new List<PvtTrialDto> { new PvtTrialDto { ChangedAt = 1000, ClickedAt = 1250 } }
        };

        _mockReoisitory.Setup(r => r.GetRecordByIdAsync(sleepRecordId)).ReturnsAsync(dummyRecord);

        var result = await _service.SubmitPvtAsync(dto);

        result.IsSuccess.Should().BeTrue();
        _mockReoisitory.Verify(r => r.UpdateRecordAsync(It.IsAny<SleepRecord>()), Times.Once);
    }

    [Fact]
    public async Task SubmitPvtAsync_更新対象の睡眠記録が存在しない場合_失敗のResultを返すこと()
    {
        var sleepRecordId = DummyId;
        var dummyRecord = CreateDummySleepRecord();
        var dto = new SubmitPvtDto
        {
            SleepRecordId = sleepRecordId.Value,
            StartTime = dummyRecord.SleepPeriod.WakeUpTime.AddMinutes(10),
            EndTime = dummyRecord.SleepPeriod.WakeUpTime.AddMinutes(13),
            Trials = new List<PvtTrialDto> { new PvtTrialDto { ChangedAt = 1000, ClickedAt = 1250 } }
        };

        _mockReoisitory.Setup(r => r.GetRecordByIdAsync(sleepRecordId)).ReturnsAsync((SleepRecord?)null);

        var result = await _service.SubmitPvtAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("更新対象");
        _mockReoisitory.Verify(repo => repo.UpdateRecordAsync(It.IsAny<SleepRecord>()), Times.Never);
    }

    [Fact]
    public async Task GetAllSleepRecordAsync_睡眠記録データが1件以上存在する場合_レコードリストを返すこと()
    {
        var expectedRecords = new List<SleepRecord>
        {
            CreateDummySleepRecord(),
            CreateDummySleepRecord()
        };
        _mockReoisitory.Setup(r => r.GetAllRecordsAsync()).ReturnsAsync(expectedRecords);

        var result = await _service.GetAllSleepRecordsAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedRecords);

        _mockReoisitory.Verify(r => r.GetAllRecordsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllSleepRecordAsync_睡眠記録データが存在しない場合_空のリストを返すこと()
    {
        _mockReoisitory.Setup(r => r.GetAllRecordsAsync()).ReturnsAsync(new List<SleepRecord>());

        var result = await _service.GetAllSleepRecordsAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockReoisitory.Verify(r => r.GetAllRecordsAsync(), Times.Once);
    }

    private static SleepRecord CreateDummySleepRecord()
    {
        var currentTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var bedtime = new DateTime(2026, 6, 12, 23, 0, 0);
        var wakeUpTime = new DateTime(2026, 6, 13, 7, 0, 0);
        var sleepPeriod = SleepPeriod.Create(bedtime, wakeUpTime).Value;

        return SleepRecord.Create(sleepPeriod, SubjectiveSleepiness.Create(5).Value, currentTime).Value;
    }
}