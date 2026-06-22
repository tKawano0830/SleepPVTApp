using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SleepPvtTracker.Core.Domain.Entities;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Infrastructure.Data;

namespace SleepPvtTracker.Infrastructure.Repositories;

public class SleepRecordRepository(AppDbContext context) : ISleepRecordRepository
{

    public async Task AddRecordAsync(SleepRecord record)
    {
        await context.SleepRecords.AddAsync(record);
        await context.SaveChangesAsync();
    }
    public async Task UpdateRecordAsync(SleepRecord record)
    {
        context.SleepRecords.Update(record);
        await context.SaveChangesAsync();
    }
    public async Task DeleteRecordAsync(SleepRecord record)
    {
        context.SleepRecords.Remove(record);
        await context.SaveChangesAsync();
    }

    public async Task<SleepRecord?> GetRecordByIdAsync(SleepRecordId id)
    {
        return await context.SleepRecords.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<SleepRecord>> GetAllRecordsAsync()
    {
        return await context.SleepRecords
            .OrderByDescending(r => r.SleepPeriod.WakeUpTime)
            .ToListAsync();
    }
}