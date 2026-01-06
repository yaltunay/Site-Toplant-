using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class DecisionService : IDecisionService
{
    private readonly ToplantiDbContext _context;

    public DecisionService(ToplantiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Decision> CreateDecisionAsync(int meetingId, string title, string description)
    {
        var decision = new Decision
        {
            MeetingId = meetingId,
            Title = title.Trim(),
            Description = description.Trim(),
            YesVotes = 0,
            NoVotes = 0,
            AbstainVotes = 0,
            IsApproved = false
        };

        _context.Decisions.Add(decision);
        await _context.SaveChangesAsync();

        return decision;
    }

    public async Task<IEnumerable<Decision>> GetDecisionsByMeetingIdAsync(int meetingId)
    {
        return await _context.Decisions
            .Include(d => d.Votes)
                .ThenInclude(v => v.Unit)
            .Include(d => d.Meeting)
            .Where(d => d.MeetingId == meetingId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteDecisionAsync(int decisionId)
    {
        var decision = await _context.Decisions.FindAsync(decisionId);
        if (decision == null) return false;

        _context.Decisions.Remove(decision);
        await _context.SaveChangesAsync();
        return true;
    }
}

