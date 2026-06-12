using System;
using System.Collections.Generic;
using System.Linq;
using SleepPvtTracker.Core.Exceptions;

namespace SleepPvtTracker.Core.ValueObjects;

//1回の試行データ
public record PvtTrial(long ChacngedAt, long ClickedAt)
{
    public long ReactionTime => ClickedAt - ChacngedAt;
    public bool IsFalseStart => ReactionTime < 0;
    ///☆遅延判定のmsは今後検討の余地あり
    public bool IsLapse => ReactionTime >= 500;
}

public record PvtResult
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    //memo:不変性を担保するためにIReadOnlyList型を採用
    //☆PvtTrialsの値比較は未実装のため、業務ロジックで必要になった時に実装すること
    public IReadOnlyList<PvtTrial> Trials { get; }
    public int ExtraFalseStarts { get; }

    //EFCore用の空コンストラクタ
    private PvtResult()
    {
        Trials = new List<PvtTrial>();
    }

    private PvtResult(DateTime startTime, DateTime endTime, List<PvtTrial> trials, int extraFalseStarts)
    {
        StartTime = startTime;
        EndTime = endTime;
        Trials = trials;
        ExtraFalseStarts = extraFalseStarts;
    }

    //memo:プリミティブ型を全てVoにするメリットが現状では少ないため、一旦ファクトリパターンでバリデーションを実装する
    //☆将来的に全部Voにしたい
    public static PvtResult Create(DateTime startTime, DateTime endTime, List<PvtTrial> trials, int extraFalseStarts)
    {
        //memo:null非許容のためプリミティブ型はnullチェックを行わない
        ArgumentNullException.ThrowIfNull(trials);

        if (endTime < startTime) throw new DomainException("終了時間は開始時間よりも未来である必要があります");
        if (extraFalseStarts < 0) throw new DomainException("無効クリック数は0以上である必要があります");

        return new PvtResult(startTime, endTime, trials, extraFalseStarts);
    }

    //以下、ビジネスルール
    //memo:計算部分を明確化するため、簡単な処理もメソッドに分離しておく

    /// <summary>
    /// 総遅延回数を取得
    /// </summary>
    public int GetTotalLapse()
    {
        return Trials.Count(p => p.IsLapse);
    }

    /// <summary>
    /// 総失敗判定回数(PvtTrials内のマイナスと無効クリックの合計)を取得
    /// </summary>
    public int GetTotalFalseStarts()
    {
        var trialFalseStarts = Trials.Count(p => p.IsFalseStart);
        return trialFalseStarts + ExtraFalseStarts;
    }

    /// <summary>
    /// 遅延と失敗を除外した有効反応の平均反応時間を取得
    /// </summary>
    public double GetAverageReactionTime()
    {
        var validTrials = Trials.Where(p => !p.IsFalseStart && !p.IsLapse).ToList();
        if (validTrials.Count == 0) return 0;
        return validTrials.Average(p => p.ReactionTime);
    }

    /// <summary>
    /// PVTスコアを100点満点で算出する
    /// 現状のルール:平均250msから10ms超過ごとに-1点、lapse-5点、falseStart-10点
    /// ☆現状は減点方式の超簡素なロジック。後々こだわること
    /// </summary>
    public int GetScore()
    {
        int score = 100;
        score -= (GetTotalLapse() * 5) + (GetTotalFalseStarts() * 10);

        var averageRT = GetAverageReactionTime();
        if (averageRT > 250) score -= (int)((averageRT - 250) / 10);

        return Math.Max(0, score);
    }
}
