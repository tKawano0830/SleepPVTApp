
namespace SleepPvtTracker.Core.UseCases.Dtos;

public class CreateSleepRecordDto
{
    public DateTime Bedtime { get; set; }
    public DateTime WakeUpTime { get; set; }
    public int SleepinessLevel { get; set; }
    public string? Comments { get; set; }
}