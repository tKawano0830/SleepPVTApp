
using SleepPvtTracker.Core.Domain.Entities;

namespace SleepPvtTracker.Core.Interfaces;

/// <summary>
/// 睡眠記録DB操作用リポジトリ
/// </summary>
public interface ISleepRecordRepository
{
    /// <summary>
    /// 新規保存
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    Task AddRecordAsync(SleepRecord record);
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    Task UpdateRecordAsync(SleepRecord record);
    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteRecordAsync(SleepRecord record);
    /// <summary>
    /// IDで1件取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<SleepRecord?> GetRecordByIdAsync(SleepRecordId id);
    /// <summary>
    /// 全ての記録を最新順で取得
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<SleepRecord>> GetAllRecordsAsync();
}