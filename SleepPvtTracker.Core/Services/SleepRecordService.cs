using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SleepPvtTracker.Core.DTOs;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Core.ValueObjects;

namespace SleepPvtTracker.Core.Services;

public class SleepRecordService(ISleepRecordRepository repository) : ISleepRecordService
{
    private readonly ISleepRecordRepository _repository = repository;

    public async Task CreateSleepRecordAsync(CreateSleepRecordDto dto)
    {
        var sleepiness = SubjectiveSleepiness.Create(dto.SleepinessLevel);
        var newRecord = SleepRecord.Create(dto.Bedtime, dto.WakeUpTime, sleepiness);

        if (!string.IsNullOrWhiteSpace(dto.Comments)) newRecord.UpdateComments(dto.Comments);

        await _repository.AddAsync(newRecord);
    }

    public async Task DeleteSleepRecordAsync(Guid id)
    {
        var record = await _repository.GetByIdAsync(id) ?? throw new ArgumentException("指定された睡眠記録が見つかりません");

        //☆削除対象のドメインルール

        await _repository.DeleteAsync(record);
    }

    public async Task SubmitPvtAsync(SubmitPvtDto dto)
    {
        var record = await _repository.GetByIdAsync(dto.SleepRecordId) ?? throw new ArgumentException("指定された睡眠記録が見つかりません");

        if (record.PvtResult != null) throw new InvalidOperationException("この睡眠記録にはすでにPVT結果が保存されています。");

        var trials = dto.Trials.Select(t => new PvtTrial(t.ChacngedAt, t.ClickedAt)).ToList();
        var pvtResult = PvtResult.Create(dto.StartTime, dto.EndTime, trials, dto.ExtraFalseStarts);

        record.RecordPvt(pvtResult);

        await _repository.UpdateAsync(record);
    }
}