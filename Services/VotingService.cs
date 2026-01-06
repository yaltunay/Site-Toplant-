using Toplanti.Models;

namespace Toplanti.Services;

public class VotingService
{
    public (int yesCount, int noCount, int abstainCount, decimal yesLandShare, decimal noLandShare, decimal abstainLandShare) CalculateVotes(
        ICollection<Vote> votes,
        ICollection<Unit> units)
    {
        var yesCount = 0;
        var noCount = 0;
        var abstainCount = 0;
        var yesLandShare = 0m;
        var noLandShare = 0m;
        var abstainLandShare = 0m;

        var unitDict = units.ToDictionary(u => u.Id);

        foreach (var vote in votes)
        {
            if (!unitDict.TryGetValue(vote.UnitId, out var unit)) continue;

            var landShare = unit.LandShare;

            switch (vote.VoteType)
            {
                case VoteType.Yes:
                    yesCount++;
                    yesLandShare += landShare;
                    break;
                case VoteType.No:
                    noCount++;
                    noLandShare += landShare;
                    break;
                case VoteType.Abstain:
                    abstainCount++;
                    abstainLandShare += landShare;
                    break;
            }
        }

        return (yesCount, noCount, abstainCount, yesLandShare, noLandShare, abstainLandShare);
    }

    public bool IsDecisionApproved(
        int yesCount,
        int noCount,
        decimal yesLandShare,
        decimal noLandShare,
        int totalAttendedUnits,
        decimal totalAttendedLandShare)
    {
        // Decision is approved if:
        // 1. More than 50% of attended units vote yes
        // 2. More than 50% of attended land share votes yes
        
        var unitMajority = (double)yesCount / totalAttendedUnits > 0.5;
        var landShareMajority = yesLandShare / totalAttendedLandShare > 0.5m;

        return unitMajority && landShareMajority;
    }
}

