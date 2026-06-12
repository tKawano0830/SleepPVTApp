using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SleepPvtTracker.Core.Entities;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Infrastructure.Data;

namespace SleepPvtTracker.Infrastructure.Repositories;

public class SleepRecordRepository(AppDbContext context) : ISleepRecordRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(SleepRecord record)
    {
        await _context.SleepRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(SleepRecord record)
    {
        _context.SleepRecords.Update(record);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(SleepRecord record)
    {
        _context.SleepRecords.Remove(record);
        await _context.SaveChangesAsync();
    }

    public async Task<SleepRecord?> GetByIdAsync(Guid id)
    {
        return await _context.SleepRecords.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<SleepRecord>> GetAllAsync()
    {
        return await _context.SleepRecords
            .OrderByDescending(r => r.WakeUpTime)
            .ToListAsync();
    }
}