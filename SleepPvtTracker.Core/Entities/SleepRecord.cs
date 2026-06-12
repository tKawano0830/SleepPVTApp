using System;
using SleepPvtTracker.Core.ValueObjects;

namespace SleepPvtTracker.Core.Entities;

//1回の睡眠をエンティティとして定義する
//memo:MainSleepをベースに作成
public class SleepRecord
{
    public Guid Id { get; init; }

    public DateTime Bedtime { get; private set; }
    public DateTime WakeUpTime { get; private set; }

    public DateOnly TargetDate => DateOnly.FromDateTime(WakeUpTime);
    public TimeSpan Duration => WakeUpTime - Bedtime;

    //memo:主観評価は睡眠時間記録とセットにして記録を強制する(数字を選ぶだけで簡単なので)
    public SubjectiveSleepiness Sleepiness { get; private set; }
    //memo:記録時に実施するとは限らないのでnull許容にする
    public PvtResult? PvtResult { get; private set; }
    //☆ここに「昼寝」や「中途覚醒あり」をメモする想定だがいずれはオブジェクト化したい
    public string Comments { get; private set; } = string.Empty;

    //EFCore用の空コンストラクタ
    private SleepRecord() { Sleepiness = null!; }

    private SleepRecord(Guid id, DateTime bedtime, DateTime wakeUpTime, SubjectiveSleepiness sleepiness)
    {
        Id = id;
        Bedtime = bedtime;
        WakeUpTime = wakeUpTime;
        Sleepiness = sleepiness;
    }

    public static SleepRecord Create(DateTime bedtime, DateTime wakeUpTime, SubjectiveSleepiness sleepiness)
    {
        ArgumentNullException.ThrowIfNull(sleepiness);

        if (wakeUpTime < bedtime) throw new ArgumentException("起床時間は就寝時間より未来である必要があります", nameof(wakeUpTime));
        if ((wakeUpTime - bedtime).TotalHours >= 24) throw new ArgumentException("1回の睡眠時間が24時間以上の記録はできません", nameof(wakeUpTime));

        return new SleepRecord(Guid.NewGuid(), bedtime, wakeUpTime, sleepiness);
    }

    public void UpdateComments(string comments)
    {
        //コメントの文字数は100文字以内にする
        //memo:これはユースケースに該当すると判断して、サービスクラスへ移行させることにした
        // if (comments?.Length > 100) throw new ArgumentException("コメントは100文字以内で入力してください", nameof(comments));

        Comments = comments ?? string.Empty;
    }

    public void RecordPvt(PvtResult pvtResult)
    {
        ArgumentNullException.ThrowIfNull(pvtResult);

        //重要なビジネスルール:PVTの実施は起床後90以内とする(パフォーマンス測定のブレを失くすため)
        var timeSinceWakeUp = pvtResult.StartTime - WakeUpTime;

        if (timeSinceWakeUp.TotalMinutes < 0) throw new InvalidOperationException("PVTの実施開始時間が起床時間より前になっています");
        if (timeSinceWakeUp.TotalMinutes > 90) throw new InvalidOperationException("PVTは起床後90以内に実施してください");

        PvtResult = pvtResult;
    }


}