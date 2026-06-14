using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SleepPvtTracker.Core.DTOs;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.Exceptions;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Core.ValueObjects;

namespace SleepPvtTracker.Core.Services;

public class SleepRecordService(ISleepRecordRepository repository) : ISleepRecordService
{
    private readonly ISleepRecordRepository _repository = repository;

    public async Task CreateSleepRecordAsync(CreateSleepRecordDto dto, DateTime currentTime)
    {
        var sleepiness = SubjectiveSleepiness.Create(dto.SleepinessLevel);
        var newRecord = SleepRecord.Create(dto.Bedtime, dto.WakeUpTime, sleepiness, currentTime);

        if (!string.IsNullOrWhiteSpace(dto.Comments)) newRecord.UpdateComments(dto.Comments);

        await _repository.AddAsync(newRecord);
    }

    public async Task DeleteSleepRecordAsync(Guid id)
    {
        var recordId = new SleepRecordId(id);
        var record = await _repository.GetByIdAsync(recordId) ?? throw new EntityNotFoundException("指定された睡眠記録が見つかりません");

        //☆削除対象のドメインルールが追加されたらここに追記

        await _repository.DeleteAsync(record);
    }

    public async Task SubmitPvtAsync(SubmitPvtDto dto)
    {
        var recordId = new SleepRecordId(dto.SleepRecordId);
        var record = await _repository.GetByIdAsync(recordId) ?? throw new EntityNotFoundException("指定された睡眠記録が見つかりません");

        if (record.PvtResult != null) throw new DomainException("この睡眠記録にはすでにPVT結果が保存されています。");

        var trials = dto.Trials.Select(t => new PvtTrial(t.ChangedAt, t.ClickedAt)).ToList();
        var pvtResult = PvtResult.Create(dto.StartTime, dto.EndTime, trials, dto.ExtraFalseStarts);

        record.RecordPvt(pvtResult);

        await _repository.UpdateAsync(record);
    }

    public async Task<IReadOnlyList<SleepRecord>> GetAllSleepRecordsAsync()
    {
        return await _repository.GetAllAsync();
    }
}