using SleepPvtTracker.Core.Common;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.Interfaces;

namespace SleepPvtTracker.Core.Domain.DomainServices;

public class SleepRecordOverlapChecker(ISleepRecordRepository repository)
{
    public async Task<Result> CheckAsync(SleepRecord sleepRecord)
    {
        var allRecords = await repository.GetAllRecordsAsync();

        var checkResult = allRecords
        .Any(r => r.SleepPeriod.IsOverlapping(sleepRecord.SleepPeriod));

        if (checkResult) return Result.Fail("睡眠時間が重複する記録があります");

        return Result.Ok();
    }
}