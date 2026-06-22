
namespace SleepPvtTracker.Core.UseCases.Dtos;

public class SubmitPvtDto
{
    public Guid SleepRecordId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int ExtraFalseStarts { get; set; }
    public List<PvtTrialDto> Trials { get; set; } = [];
}

public class PvtTrialDto
{
    public long ChangedAt { get; set; }
    public long ClickedAt { get; set; }
}