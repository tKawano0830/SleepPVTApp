
using System.ComponentModel.DataAnnotations;
using SleepPvtTracker.Core.DTOs;

namespace SleepPvtTracker.Web.Models;

public class CreateSleepRecordViewModel
{
    [Required]
    [Display(Name = "就寝時間")]
    public DateTime? Bedtime { get; set; }
    [Required]
    [Display(Name = "起床時間")]
    public DateTime? WakeUpTime { get; set; }

    [Required]
    [Range(1, 9, ErrorMessage = "{0}は{1}～{2}の間で指定してください。")]
    [Display(Name = "眠気レベル")]
    public int? SleepinessLevel { get; set; }

    //memo:運用都合上100文字制限とする
    [StringLength(100, ErrorMessage = "{0}は{1}文字以内で指定してください。")]
    [Display(Name = "備考")]
    public string? Comments { get; set; }

    public CreateSleepRecordDto ConvertToDto()
    {
        return new CreateSleepRecordDto
        {
            Bedtime = this.Bedtime!.Value,
            WakeUpTime = this.WakeUpTime!.Value,
            SleepinessLevel = this.SleepinessLevel!.Value,
            Comments = this.Comments
        };
    }

}