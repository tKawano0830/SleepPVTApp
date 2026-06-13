using System.ComponentModel.DataAnnotations;
using SleepPvtTracker.Core.DTOs;

namespace SleepPvtTracker.Web.Models;

public class SubmitPvtViewModel
{
    [Required]
    [Display(Name = "睡眠記録ID")]
    public Guid? SleepRecordId { get; set; }

    [Required]
    [Display(Name = "開始時間")]
    public DateTime? StartTime { get; set; }
    [Required]
    [Display(Name = "終了時間")]
    public DateTime? EndTime { get; set; }
    [Required]
    [Display(Name = "無効クリック数")]
    [Range(0, int.MaxValue, ErrorMessage = "{0}がマイナスです")]
    public int? ExtraFalseStarts { get; set; }

    [Required]
    [Display(Name = "Pvt実施結果")]
    public List<PvtTrialViewModel> Trials { get; set; } = [];

    public SubmitPvtDto ConvertToDto()
    {
        var localStartTime = StartTime!.Value.ToLocalTime();
        var localEndTime = EndTime!.Value.ToLocalTime();

        return new SubmitPvtDto
        {
            SleepRecordId = SleepRecordId!.Value,
            StartTime = localStartTime,
            EndTime = localEndTime,
            ExtraFalseStarts = ExtraFalseStarts!.Value,
            Trials = Trials.Select(t => new PvtTrialDto { ChangedAt = t.ChangedAt!.Value, ClickedAt = t.ClickedAt!.Value }).ToList()
        };
    }
}

public class PvtTrialViewModel
{
    [Required]
    public long? ChangedAt { get; set; }
    [Required]
    public long? ClickedAt { get; set; }
}