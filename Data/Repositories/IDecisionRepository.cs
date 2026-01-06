using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Decision entity için özel repository interface
/// </summary>
public interface IDecisionRepository : IRepository<Decision>
{
    /// <summary>
    /// Meeting ID'ye göre kararları getirir
    /// </summary>
    Task<IEnumerable<Decision>> GetDecisionsByMeetingIdAsync(int meetingId);

    /// <summary>
    /// Meeting ID'ye göre kararları oylarla birlikte getirir
    /// </summary>
    Task<IEnumerable<Decision>> GetDecisionsWithVotesByMeetingIdAsync(int meetingId);

    /// <summary>
    /// Meeting ID listesine göre karar sayısını getirir
    /// </summary>
    Task<int> CountDecisionsByMeetingIdsAsync(IEnumerable<int> meetingIds);
}
