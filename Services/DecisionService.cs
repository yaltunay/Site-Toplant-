using Toplanti.Data;
using Toplanti.Infrastructure.Mappings;
using Toplanti.Infrastructure.Validation;
using Toplanti.Models;
using Toplanti.Models.DTOs;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class DecisionService : IDecisionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ValidationService _validationService;

    public DecisionService(IUnitOfWork unitOfWork, ValidationService validationService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
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

        // Validate decision
        var validationResult = _validationService.ValidateDecision(decision);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(validationResult.ErrorMessage ?? "Validation failed");
        }

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

