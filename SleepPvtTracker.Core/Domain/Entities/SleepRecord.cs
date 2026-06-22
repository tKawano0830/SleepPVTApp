using SleepPvtTracker.Core.Common;
using SleepPvtTracker.Core.Domain.ValueObjects;

namespace SleepPvtTracker.Core.Domain.Entities;

public readonly record struct SleepRecordId(Guid Value);

//1回の睡眠をエンティティとして定義する
//memo:MainSleepをベースに作成
public class SleepRecord
{
    public const int PvtAvailableMinutes = 90;
    public const int MaxCommentsLength = 100;

    public SleepRecordId Id { get; init; }

    public SleepPeriod SleepPeriod { get; }

    public DateOnly TargetDate => DateOnly.FromDateTime(SleepPeriod.WakeUpTime);

    //memo:主観評価は睡眠時間記録とセットにして記録を強制する(数字を選ぶだけで簡単なので)
    public SubjectiveSleepiness Sleepiness { get; }
    //memo:記録時に実施するとは限らないのでnull許容にする
    public PvtResult? PvtResult { get; private set; }
    //☆ここに「昼寝」や「中途覚醒あり」をメモする想定だがいずれはオブジェクト化したい
    public string Comments { get; private set; } = string.Empty;

    public bool IsPvtAvailable(DateTime currentTime) => PvtResult == null && (currentTime - SleepPeriod.WakeUpTime).TotalMinutes <= PvtAvailableMinutes;

    //EFCore用の空コンストラクタ
    private SleepRecord() { SleepPeriod = null!; Sleepiness = null!; }

    private SleepRecord(SleepRecordId id, SleepPeriod sleepPeriod, SubjectiveSleepiness sleepiness)
    {
        Id = id;
        SleepPeriod = sleepPeriod;
        Sleepiness = sleepiness;
    }

    public static Result<SleepRecord> Create(SleepPeriod sleepPeriod, SubjectiveSleepiness sleepiness, DateTime currentTime)
    {
        ArgumentNullException.ThrowIfNull(sleepPeriod);
        ArgumentNullException.ThrowIfNull(sleepiness);

        if (sleepPeriod.WakeUpTime > currentTime) return Result<SleepRecord>.Fail("起床時間が未来に設定されています");

        return Result<SleepRecord>.Ok(new SleepRecord(new SleepRecordId(Guid.NewGuid()), sleepPeriod, sleepiness));
    }

    public Result UpdateComments(string comments)
    {
        //コメントの文字数は100文字以内にする(日記ではなく軽いメモとして利用して欲しいため)
        if (comments?.Length > MaxCommentsLength) return Result.Fail($"コメントは{MaxCommentsLength}文字以内で入力してください");

        Comments = comments ?? string.Empty;
        return Result.Ok();
    }

    public Result RecordPvt(PvtResult pvtResult)
    {
        ArgumentNullException.ThrowIfNull(pvtResult);

        //重要なビジネスルール:PVTの実施は起床後90以内とする(パフォーマンス測定のブレを失くすため)
        var timeSinceWakeUp = pvtResult.StartTime - SleepPeriod.WakeUpTime;
        if (timeSinceWakeUp.TotalMinutes < 0) return Result.Fail("PVTの実施開始時間が起床時間より前になっています");
        if (timeSinceWakeUp.TotalMinutes > PvtAvailableMinutes) return Result.Fail($"PVTは起床後{PvtAvailableMinutes}分以内に実施してください");

        PvtResult = pvtResult;
        return Result.Ok();
    }


}