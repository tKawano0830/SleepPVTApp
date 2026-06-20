using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SleepPvtTracker.Core.Common;
using SleepPvtTracker.Core.DTOs;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.Exceptions;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Core.ValueObjects;

namespace SleepPvtTracker.Core.Services;

public class SleepRecordService(ISleepRecordRepository repository) : ISleepRecordService
{
    private readonly ISleepRecordRepository _repository = repository;

    public async Task<Result> CreateSleepRecordAsync(CreateSleepRecordDto dto, DateTime currentTime)
    {
        var sleepinessResult = SubjectiveSleepiness.Create(dto.SleepinessLevel);
        if (sleepinessResult.IsFailure) return Result.Fail(sleepinessResult.ErrorMessage);

        var recordResult = SleepRecord.Create(dto.Bedtime, dto.WakeUpTime, sleepinessResult.Value, currentTime);
        if (recordResult.IsFailure) return Result.Fail(recordResult.ErrorMessage);

        if (!string.IsNullOrWhiteSpace(dto.Comments))
        {
            var result = recordResult.Value.UpdateComments(dto.Comments);
            if (result.IsFailure) return Result.Fail(result.ErrorMessage);
        }

        await _repository.AddAsync(recordResult.Value);
        return Result.Ok();
    }

    public async Task<Result> DeleteSleepRecordAsync(Guid id)
    {
        var record = await _repository.GetByIdAsync(new SleepRecordId(id));
        if (record == null) return Result.Fail("削除対象の睡眠記録が見つかりませんでした");

        //☆削除対象のドメインルールが追加されたらここに追記

        await _repository.DeleteAsync(record);
        return Result.Ok();
    }

    public async Task<Result> SubmitPvtAsync(SubmitPvtDto dto)
    {
        var record = await _repository.GetByIdAsync(new SleepRecordId(dto.SleepRecordId));
        if (record == null) return Result.Fail("更新対象の睡眠記録が見つかりませんでした");

        if (record.PvtResult != null) return Result.Fail("この睡眠記録にはすでにPVT結果が保存されています");

        var trials = dto.Trials.Select(t => new PvtTrial(t.ChangedAt, t.ClickedAt)).ToList();
        var pvtResult = PvtResult.Create(dto.StartTime, dto.EndTime, trials, dto.ExtraFalseStarts);
        if (pvtResult.IsFailure) return Result.Fail(pvtResult.ErrorMessage);

        var recordPvtResult = record.RecordPvt(pvtResult.Value);
        if (recordPvtResult.IsFailure) return Result.Fail(recordPvtResult.ErrorMessage);

        await _repository.UpdateAsync(record);
        return Result.Ok();
    }

    public async Task<IReadOnlyList<SleepRecord>> GetAllSleepRecordsAsync()
    {
        return await _repository.GetAllAsync();
    }
}