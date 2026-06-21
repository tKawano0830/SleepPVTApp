using SleepPvtTracker.Core.Common;

public record SleepPeriod
{
    public DateTime Bedtime { get; }
    public DateTime WakeUpTime { get; }
    public TimeSpan Duration => WakeUpTime - Bedtime;

    private SleepPeriod(DateTime bedtime, DateTime wakeUpTime)
    {
        Bedtime = bedtime;
        WakeUpTime = wakeUpTime;
    }
    public static Result<SleepPeriod> Create(DateTime bedtime, DateTime wakeUpTime)
    {
        if (bedtime > wakeUpTime) return Result<SleepPeriod>.Fail("起床時間は就寝時間より未来である必要があります");
        if ((wakeUpTime - bedtime).TotalHours >= 24) return Result<SleepPeriod>.Fail("睡眠時間が24時間以上です");

        return Result<SleepPeriod>.Ok(new SleepPeriod(bedtime, wakeUpTime));
    }
}
