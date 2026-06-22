using SleepPvtTracker.Core.Common;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.UseCases.Dtos;

namespace SleepPvtTracker.Core.UseCases;

/// <summary>
/// 睡眠時間記録用ユースケースインタフェース
/// </summary>
public interface ISleepRecordUseCase
{
    /// <summary>
    /// 睡眠記録を新規作成する
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<Result> CreateSleepRecordAsync(CreateSleepRecordDto dto, DateTime dateTime);

    /// <summary>
    /// 睡眠記録を削除する
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ///memo:冗長になるためDtoは作成しない
    Task<Result> DeleteSleepRecordAsync(Guid id);

    /// <summary>
    /// PVT結果を提出し、該当する睡眠記録に紐づけて保存する
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<Result> SubmitPvtAsync(SubmitPvtDto dto);

    /// <summary>
    /// DBから全ての睡眠記録を取得する
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<SleepRecord>> GetAllSleepRecordsAsync();
    //memo:即時実行のコレクションを返す(Controller側でDB操作させないため)
}