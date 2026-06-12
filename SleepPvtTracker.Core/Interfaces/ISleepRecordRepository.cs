using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SleepPvtTracker.Core.Entities;

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
    Task AddAsync(SleepRecord record);
    /// <summary>
    /// IDで1件取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<SleepRecord?> GetByIdAsync(Guid id);
    /// <summary>
    /// 全ての記録を最新順で取得
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<SleepRecord>> GetAllAsync();
}