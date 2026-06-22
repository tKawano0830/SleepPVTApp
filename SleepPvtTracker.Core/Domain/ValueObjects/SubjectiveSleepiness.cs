using SleepPvtTracker.Core.Common;

namespace SleepPvtTracker.Core.Domain.ValueObjects;

/// <summary>
/// 眠気の主観的評価
/// </summary>
public record SubjectiveSleepiness
{
    //主観的な眠気のレベル(1:非常にはっきり目覚めている～9：とても眠い)
    public int Level { get; }

    private SubjectiveSleepiness(int level)
    {
        Level = level;
    }

    public static Result<SubjectiveSleepiness> Create(int level)
    {
        //カロリンスカ眠気尺度(KSS)を採用
        if (level < 1 || level > 9) return Result<SubjectiveSleepiness>.Fail("眠気レベルは1から9の範囲で指定してください");

        return Result<SubjectiveSleepiness>.Ok(new SubjectiveSleepiness(level));
    }
}