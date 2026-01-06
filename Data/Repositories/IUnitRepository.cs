using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Unit entity için özel repository interface
/// </summary>
public interface IUnitRepository : IRepository<Unit>
{
    /// <summary>
    /// Site ID'ye göre birimleri getirir
    /// </summary>
    Task<IEnumerable<Unit>> GetUnitsBySiteIdAsync(int siteId);

    /// <summary>
    /// Site ID'ye göre aktif birimleri getirir
    /// </summary>
    Task<IEnumerable<Unit>> GetActiveUnitsBySiteIdAsync(int siteId);

    /// <summary>
    /// Site ID'ye göre aktif birim sayısını getirir
    /// </summary>
    Task<int> GetActiveUnitCountBySiteIdAsync(int siteId);
}

