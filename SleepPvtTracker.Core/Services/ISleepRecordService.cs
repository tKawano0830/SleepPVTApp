using System.Threading.Tasks;
using SleepPvtTracker.Core.DTOs;

namespace SleepPvtTracker.Core.Services;

/// <summary>
/// 睡眠時間記録用ユースケースインタフェース
/// </summary>
public interface ISleepRecordService
{
    /// <summary>
    /// 睡眠記録を新規作成する
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task CreateSleepRecordAsync(CreateSleepRecordDto dto);

    /// <summary>
    /// 睡眠記録を削除する
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ///memo:冗長になるためDtoは作成しない
    Task DeleteSleepRecordAsync(Guid id);

    /// <summary>
    /// PVT結果を提出し、該当する睡眠記録に紐づけて保存する
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task SubmitPvtAsync(SubmitPvtDto dto);
}