using System;

namespace SleepPvtTracker.Core.ValueObjects;

//ナップやうたた寝を含まず、きちんとベッドに横になって寝た時間として定義
//※MainSleepは完全に撤廃しましたがドキュメントとして残します。
internal record MainSleep
{
    public DateTime Bedtime { get; }
    public DateTime WakeUpTime { get; }
    public TimeSpan Duration => WakeUpTime - Bedtime;

    private MainSleep(DateTime bedtime, DateTime wakeupTime)
    {
        Bedtime = bedtime;
        WakeUpTime = wakeupTime;
    }

    public static MainSleep Create(DateTime bedtime, DateTime wakeUpTime)
    {
        if (wakeUpTime < bedtime) throw new ArgumentException("起床時間は就寝時間より未来である必要があります", nameof(wakeUpTime));
        if ((wakeUpTime - bedtime).TotalHours >= 24) throw new ArgumentException("1回の睡眠時間が24時間以上の記録はできません", nameof(wakeUpTime));

        return new MainSleep(bedtime, wakeUpTime);
    }
}