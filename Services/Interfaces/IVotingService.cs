using Toplanti.Models;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Oylama işlemlerini yöneten servis arayüzü
/// </summary>
public interface IVotingService
{
    (int yesCount, int noCount, int abstainCount, decimal yesLandShare, decimal noLandShare, decimal abstainLandShare) 
        CalculateVotes(ICollection<Vote> votes, ICollection<Unit> units);
    
    bool IsDecisionApproved(
        int yesCount,
        int noCount,
        decimal yesLandShare,
        decimal noLandShare,
        int totalAttendedUnits,
        decimal totalAttendedLandShare);
}

