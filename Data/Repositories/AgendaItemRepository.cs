using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// AgendaItem entity için repository implementation
/// </summary>
public class AgendaItemRepository : Repository<AgendaItem>, IAgendaItemRepository
{
    public AgendaItemRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AgendaItem>> GetAgendaItemsByMeetingIdAsync(int meetingId)
    {
        return await _dbSet
            .Where(a => a.MeetingId == meetingId)
            .OrderBy(a => a.Order)
            .ToListAsync();
    }
}

