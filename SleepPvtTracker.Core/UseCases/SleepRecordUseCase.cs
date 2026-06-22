using SleepPvtTracker.Core.Common;
using SleepPvtTracker.Core.UseCases.Dtos;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.Domain.ValueObjects;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Core.Domain.DomainServices;

namespace SleepPvtTracker.Core.UseCases;

public class SleepRecordUseCase(ISleepRecordRepository repository, SleepRecordOverlapChecker overlapChecker) : ISleepRecordUseCase
{
    public async Task<Result> CreateSleepRecordAsync(CreateSleepRecordDto dto, DateTime currentTime)
    {
        var sleepinessResult = SubjectiveSleepiness.Create(dto.SleepinessLevel);
        if (sleepinessResult.IsFailure) return Result.Fail(sleepinessResult.ErrorMessage);

        var sleepPeriodResult = SleepPeriod.Create(dto.Bedtime, dto.WakeUpTime);
        if (sleepPeriodResult.IsFailure) return Result.Fail(sleepPeriodResult.ErrorMessage);

        var recordResult = SleepRecord.Create(sleepPeriodResult.Value, sleepinessResult.Value, currentTime);
        if (recordResult.IsFailure) return Result.Fail(recordResult.ErrorMessage);

        if (!string.IsNullOrWhiteSpace(dto.Comments))
        {
            var result = recordResult.Value.UpdateComments(dto.Comments);
            if (result.IsFailure) return Result.Fail(result.ErrorMessage);
        }

        //重複ルールのチェックをドメインサービスクラスに依頼
        var checkResult = await overlapChecker.CheckAsync(recordResult.Value);
        if (checkResult.IsFailure) return Result.Fail(checkResult.ErrorMessage);

        await repository.AddRecordAsync(recordResult.Value);
        return Result.Ok();
    }

    public async Task<Result> DeleteSleepRecordAsync(Guid id)
    {
        var record = await repository.GetRecordByIdAsync(new SleepRecordId(id));
        if (record == null) return Result.Fail("削除対象の睡眠記録が見つかりませんでした");

        await repository.DeleteRecordAsync(record);
        return Result.Ok();
    }

    public async Task<Result> SubmitPvtAsync(SubmitPvtDto dto)
    {
        var record = await repository.GetRecordByIdAsync(new SleepRecordId(dto.SleepRecordId));
        if (record == null) return Result.Fail("更新対象の睡眠記録が見つかりませんでした");

        if (record.PvtResult != null) return Result.Fail("この睡眠記録にはすでにPVT結果が保存されています");

        var trials = dto.Trials.Select(t => new PvtTrial(t.ChangedAt, t.ClickedAt)).ToList();
        var pvtResult = PvtResult.Create(dto.StartTime, dto.EndTime, trials, dto.ExtraFalseStarts);
        if (pvtResult.IsFailure) return Result.Fail(pvtResult.ErrorMessage);

        var recordPvtResult = record.RecordPvt(pvtResult.Value);
        if (recordPvtResult.IsFailure) return Result.Fail(recordPvtResult.ErrorMessage);

        await repository.UpdateRecordAsync(record);
        return Result.Ok();
    }

    public async Task<IReadOnlyList<SleepRecord>> GetAllSleepRecordsAsync()
    {
        return await repository.GetAllRecordsAsync();
    }
}