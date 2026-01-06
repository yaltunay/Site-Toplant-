using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Meeting entity için repository implementation
/// </summary>
public class MeetingRepository : Repository<Meeting>, IMeetingRepository
{
    public MeetingRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Meeting>> GetMeetingsBySiteIdAsync(int siteId)
    {
        var siteUnitIds = await _context.Units
            .Where(u => u.SiteId == siteId && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync();

        return await _dbSet
            .Include(m => m.Attendances)
            .Where(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || !m.Attendances.Any())
            .OrderByDescending(m => m.MeetingDate)
            .ToListAsync();
    }

    public async Task<Meeting?> GetMeetingWithDetailsAsync(int meetingId)
    {
        return await _dbSet
            .Include(m => m.Attendances)
                .ThenInclude(a => a.Unit)
                    .ThenInclude(u => u!.UnitType)
            .Include(m => m.Proxies)
                .ThenInclude(p => p.GiverUnit)
            .Include(m => m.Proxies)
                .ThenInclude(p => p.ReceiverUnit)
            .Include(m => m.AgendaItems)
            .Include(m => m.Documents)
            .Include(m => m.Decisions)
            .FirstOrDefaultAsync(m => m.Id == meetingId);
    }

    public async Task<int> GetActiveUnitCountBySiteIdAsync(int siteId)
    {
        return await _context.Units.CountAsync(u => u.SiteId == siteId && u.IsActive);
    }
}

