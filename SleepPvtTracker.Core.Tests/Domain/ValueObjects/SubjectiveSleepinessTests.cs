using System;
using FluentAssertions;
using SleepPvtTracker.Core.Domain.ValueObjects;
using Xunit;

namespace SleepPvtTracker.Core.Domain.ValueObjects;

public class SubjectiveSleepinessTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void Create_1から9の値を渡した場合_正しくインスタンスが生成されプロパティがセットされていること(int validLevel)
    {
        var sleepiness = SubjectiveSleepiness.Create(validLevel);

        sleepiness.IsSuccess.Should().BeTrue();
        sleepiness.Value.Should().NotBeNull();
        sleepiness.Value.Level.Should().Be(validLevel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-5)]
    public void Create_1から9以外の値を渡した場合_失敗のResultを返すこと(int invalidLevel)
    {
        var sleepiness = SubjectiveSleepiness.Create(invalidLevel);

        sleepiness.IsFailure.Should().BeTrue();
        sleepiness.ErrorMessage.Should().Contain("眠気レベル");
    }
}