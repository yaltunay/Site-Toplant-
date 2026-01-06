using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// MeetingAttendance entity için özel repository interface
/// </summary>
public interface IMeetingAttendanceRepository : IRepository<MeetingAttendance>
{
    /// <summary>
    /// Unit ID listesine göre meeting ID'leri getirir
    /// </summary>
    Task<IEnumerable<int>> GetMeetingIdsByUnitIdsAsync(IEnumerable<int> unitIds);
}

