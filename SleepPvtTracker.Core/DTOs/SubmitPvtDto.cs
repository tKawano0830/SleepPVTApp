using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SleepPvtTracker.Core.DTOs;

public class SubmitPvtDto
{
    [Required]
    public Guid SleepRecordId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    [Required]
    public int ExtraFalseStarts { get; set; }

    [Required]
    public List<PvtTrialDto> Trials { get; set; } = [];
}

public class PvtTrialDto
{
    [Required]
    public long ChacngedAt { get; set; }
    [Required]
    public long ClickedAt { get; set; }
}