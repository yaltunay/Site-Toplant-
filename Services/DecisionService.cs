using Toplanti.Data;
using Toplanti.Data.Repositories;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class DecisionService : IDecisionService
{
    private readonly IDecisionRepository _decisionRepository;
    private readonly ToplantiDbContext _context;

    public DecisionService(IDecisionRepository decisionRepository, ToplantiDbContext context)
    {
        _decisionRepository = decisionRepository ?? throw new ArgumentNullException(nameof(decisionRepository));
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

        await _decisionRepository.AddAsync(decision);
        await _context.SaveChangesAsync();

        return decision;
    }

    public async Task<IEnumerable<Decision>> GetDecisionsByMeetingIdAsync(int meetingId)
    {
        return await _decisionRepository.GetDecisionsWithVotesByMeetingIdAsync(meetingId);
    }

    public async Task<bool> DeleteDecisionAsync(int decisionId)
    {
        var decision = await _decisionRepository.GetByIdAsync(decisionId);
        if (decision == null) return false;

        _decisionRepository.Remove(decision);
        await _context.SaveChangesAsync();
        return true;
    }
}

