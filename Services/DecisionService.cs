using Toplanti.Data;
using Toplanti.Infrastructure.Mappings;
using Toplanti.Models;
using Toplanti.Models.DTOs;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class DecisionService : IDecisionService
{
    private readonly IUnitOfWork _unitOfWork;

    public DecisionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<DecisionDto> CreateDecisionAsync(int meetingId, string title, string description)
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

        // Reload with navigation properties for mapping
        var decisionWithDetails = await _unitOfWork.Decisions.GetDecisionsWithVotesByMeetingIdAsync(meetingId);
        var createdDecision = decisionWithDetails.FirstOrDefault(d => d.Id == decision.Id);
        
        return createdDecision != null ? EntityMapper.ToDto(createdDecision) : EntityMapper.ToDto(decision);
    }

    public async Task<IEnumerable<DecisionDto>> GetDecisionsByMeetingIdAsync(int meetingId)
    {
        var decisions = await _unitOfWork.Decisions.GetDecisionsWithVotesByMeetingIdAsync(meetingId);
        return EntityMapper.ToDto(decisions);
    }

    public async Task<bool> DeleteDecisionAsync(int decisionId)
    {
        var decision = await _unitOfWork.Decisions.GetByIdAsync(decisionId);
        if (decision == null) return false;

        _unitOfWork.Decisions.Remove(decision);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<Decision?> GetDecisionDomainModelByIdAsync(int decisionId)
    {
        var decisions = await _unitOfWork.Decisions.GetDecisionsWithVotesByMeetingIdAsync(0);
        return decisions.FirstOrDefault(d => d.Id == decisionId);
    }
}

