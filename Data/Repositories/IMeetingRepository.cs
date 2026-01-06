using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Meeting entity için özel repository interface
/// </summary>
public interface IMeetingRepository : IRepository<Meeting>
{
    /// <summary>
    /// Site ID'ye göre toplantıları getirir
    /// </summary>
    Task<IEnumerable<Meeting>> GetMeetingsBySiteIdAsync(int siteId);

    /// <summary>
    /// ID'ye göre toplantıyı tüm ilişkileriyle getirir
    /// </summary>
    Task<Meeting?> GetMeetingWithDetailsAsync(int meetingId);

    /// <summary>
    /// Site ID'ye göre aktif birim sayısını getirir
    /// </summary>
    Task<int> GetActiveUnitCountBySiteIdAsync(int siteId);
}

