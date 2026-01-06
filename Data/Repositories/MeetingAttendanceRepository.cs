using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// MeetingAttendance entity için repository implementation
/// </summary>
public class MeetingAttendanceRepository : Repository<MeetingAttendance>, IMeetingAttendanceRepository
{
    public MeetingAttendanceRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<int>> GetMeetingIdsByUnitIdsAsync(IEnumerable<int> unitIds)
    {
        var unitIdList = unitIds.ToList();
        if (!unitIdList.Any())
            return Enumerable.Empty<int>();

        return await _dbSet
            .Where(a => unitIdList.Contains(a.UnitId))
            .Select(a => a.MeetingId)
            .Distinct()
            .ToListAsync();
    }
}

