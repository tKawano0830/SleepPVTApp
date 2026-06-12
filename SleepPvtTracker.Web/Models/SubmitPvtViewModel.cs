using System.ComponentModel.DataAnnotations;

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
}

public class PvtTrialViewModel
{
    [Required]
    public long? ChangedAt { get; set; }
    [Required]
    public long? ClickedAt { get; set; }
}