using SleepPvtTracker.Core.Entities;

namespace SleepPvtTracker.Web.Models;

public class SleepRecordListViewModel
{
    public Guid Id { get; set; }
    public string TargetDate { get; set; } = string.Empty;
    public string Bedtime { get; set; } = string.Empty;
    public string WakeUpTime { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public int SleepinessLevel { get; set; }
    public bool HasPvt { get; set; }
    public bool IsPvtAvailable { get; set; }
    public int? PvtScore { get; set; }

    public static SleepRecordListViewModel ConvertFromEntity(SleepRecord entity)
    {
        return new SleepRecordListViewModel
        {
            Id = entity.Id.Value,
            TargetDate = entity.TargetDate.ToString("yyyy/MM/dd"),
            Bedtime = entity.Bedtime.ToString("MM/dd HH:mm"),
            WakeUpTime = entity.WakeUpTime.ToString("MM/dd HH:mm"),
            Comments = entity.Comments,
            Duration = entity.Duration.ToString(@"hh\:mm"),
            SleepinessLevel = entity.Sleepiness.Level,
            HasPvt = entity.PvtResult != null,
            IsPvtAvailable = entity.IsPvtAvailable(DateTime.Now),
            PvtScore = entity.PvtResult?.GetScore()
        };
    }
}