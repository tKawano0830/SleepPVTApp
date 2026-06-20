using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using SleepPvtTracker.Core.Common;
using SleepPvtTracker.Core.Exceptions;
using SleepPvtTracker.Core.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.Tests.ValueObjects;

public class PvtResultTests
{
    [Fact]
    public void Create_正常な値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること()
    {
        var startTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var endTime = new DateTime(2026, 6, 13, 8, 3, 0);
        var pvtTrials = new List<PvtTrial> { new PvtTrial(100, 1250) };
        var extraFalseStarts = 2;

        var result = PvtResult.Create(startTime, endTime, pvtTrials, extraFalseStarts);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StartTime.Should().Be(startTime);
        result.Value.EndTime.Should().Be(endTime);
        //オブジェクトの同等比較
        result.Value.Trials.Should().BeEquivalentTo(pvtTrials);
        result.Value.ExtraFalseStarts.Should().Be(extraFalseStarts);
    }

    [Fact]
    public void Create_Nullのコレクションが渡された場合_例外をスローすること()
    {
        Action act = () => PvtResult.Create(new DateTime(2026, 6, 13, 8, 0, 0), new DateTime(2026, 6, 13, 8, 3, 0), null!, 0);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_不正な時間関係が渡された場合_失敗のResultを返すこと()
    {
        var startTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var endTime = new DateTime(2026, 6, 13, 7, 59, 0);
        var pvtTrials = new List<PvtTrial>();

        var result = PvtResult.Create(startTime, endTime, pvtTrials, 0);

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("終了時間");
    }

    [Fact]
    public void GetTotalLapse_500ms以上の反応を遅延としてカウントして返すこと()
    {
        var pvtTrials = new List<PvtTrial>
        {
            new PvtTrial(1000,1499),
            new PvtTrial(2000,2500),//遅延
            new PvtTrial(3000,4000)//遅延
        };
        var result = CreateTestObject(pvtTrials);

        var totalLapse = result.GetTotalLapse();
        totalLapse.Should().Be(2);
    }

    [Fact]
    public void GetTotalFalseStarts_マイナスの反応とExtraFalseStartsを合算して返すこと()
    {
        var pvtTrials = new List<PvtTrial>
        {
            new PvtTrial(1000,900),//マイナス反応
            new PvtTrial(2000,2200)
        };
        var extraFalseStarts = 3;
        var result = CreateTestObject(pvtTrials, extraFalseStarts);

        var totalFolseStarts = result.GetTotalFalseStarts();
        totalFolseStarts.Should().Be(4);
    }

    [Fact]
    public void GetAverageReactionTime_LapseとFalseStartを除外した平均反応速度を返すこと()
    {
        var pvtTrials = new List<PvtTrial>
        {
            new PvtTrial(1000,900),//除外(FalseStart)
            new PvtTrial(2000,2200),//200ms
            new PvtTrial(3000,3100),//100ms
            new PvtTrial(4000,4600)//除外(Lapse)
        };
        var result = CreateTestObject(pvtTrials);

        var averageRT = result.GetAverageReactionTime();
        averageRT.Should().Be(150);//(200+100)/2
    }

    [Fact]
    public void GetAverageReactionTime_有効なPvtTrialが一つもない場合は0を返すこと()
    {
        var pvtTrials = new List<PvtTrial>
        {
            new PvtTrial(1000,900),//除外(FalseStart)
            new PvtTrial(2000,1999),//除外(FalseStart)
            new PvtTrial(3000,3500),//除外(Lapse)
            new PvtTrial(4000,4600)//除外(Lapse)
        };
        var result = CreateTestObject(pvtTrials);

        var averageRT = result.GetAverageReactionTime();
        averageRT.Should().Be(0);
    }

    [Fact]
    public void GetScore_ルール通りにスコアが算出されること()
    {
        var pvtTrials = new List<PvtTrial>
        {
            new PvtTrial(1000,1260),//平均260ms(-1点)
            new PvtTrial(2000,2500),//lapse-5点
            new PvtTrial(3000,2900)//falseStart-10点
        };
        var extraFalseStarts = 1;//-10点

        var result = CreateTestObject(pvtTrials, extraFalseStarts);

        var score = result.GetScore();
        score.Should().Be(74);
    }

    private static PvtResult CreateTestObject(List<PvtTrial> pvtTrials, int extraFalseStarts = 0)
    {
        var startTime = new DateTime(2026, 6, 13, 8, 0, 0);
        var endTime = new DateTime(2026, 6, 13, 8, 3, 0);
        var result = PvtResult.Create(startTime, endTime, pvtTrials, extraFalseStarts);
        return result.Value;
    }

}