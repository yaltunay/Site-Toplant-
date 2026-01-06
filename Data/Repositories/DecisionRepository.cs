using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Decision entity için repository implementation
/// </summary>
public class DecisionRepository : Repository<Decision>, IDecisionRepository
{
    public DecisionRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Decision>> GetDecisionsByMeetingIdAsync(int meetingId)
    {
        return await _dbSet
            .Where(d => d.MeetingId == meetingId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Decision>> GetDecisionsWithVotesByMeetingIdAsync(int meetingId)
    {
        return await _dbSet
            .Include(d => d.Votes)
                .ThenInclude(v => v.Unit)
            .Include(d => d.Meeting)
            .Where(d => d.MeetingId == meetingId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> CountDecisionsByMeetingIdsAsync(IEnumerable<int> meetingIds)
    {
        var meetingIdList = meetingIds.ToList();
        if (!meetingIdList.Any())
            return 0;

        return await _dbSet.CountAsync(d => meetingIdList.Contains(d.MeetingId));
    }
}

