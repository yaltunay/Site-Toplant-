using Toplanti.Data;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class DecisionService : IDecisionService
{
    private readonly IUnitOfWork _unitOfWork;

    public DecisionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

        await _unitOfWork.Decisions.AddAsync(decision);
        await _unitOfWork.SaveChangesAsync();

        return decision;
    }

    public async Task<IEnumerable<Decision>> GetDecisionsByMeetingIdAsync(int meetingId)
    {
        return await _unitOfWork.Decisions.GetDecisionsWithVotesByMeetingIdAsync(meetingId);
    }

    public async Task<bool> DeleteDecisionAsync(int decisionId)
    {
        var decision = await _unitOfWork.Decisions.GetByIdAsync(decisionId);
        if (decision == null) return false;

        _unitOfWork.Decisions.Remove(decision);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

