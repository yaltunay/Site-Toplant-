using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// AgendaItem entity için özel repository interface
/// </summary>
public interface IAgendaItemRepository : IRepository<AgendaItem>
{
    /// <summary>
    /// Meeting ID'ye göre gündem maddelerini sırayla getirir
    /// </summary>
    Task<IEnumerable<AgendaItem>> GetAgendaItemsByMeetingIdAsync(int meetingId);
}

