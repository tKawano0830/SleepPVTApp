using System;
using FluentAssertions;
using SleepPvtTracker.Core.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.ValueObjects;

public class SubjectiveSleepinessTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void Create_1から9の値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること(int validLevel)
    {
        var sleepiness = SubjectiveSleepiness.Create(validLevel);

        sleepiness.Should().NotBeNull();
        sleepiness.Level.Should().Be(validLevel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-5)]
    public void Create_1から9以外の値を渡した場合_例外をスローすること(int invalidLevel)
    {
        Action act = () => SubjectiveSleepiness.Create(invalidLevel);

        act.Should().Throw<ArgumentException>().WithMessage("*眠気レベル*");
    }
}