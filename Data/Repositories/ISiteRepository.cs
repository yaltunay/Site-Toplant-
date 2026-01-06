using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Site entity için özel repository interface
/// </summary>
public interface ISiteRepository : IRepository<Site>
{
    /// <summary>
    /// Tüm siteleri alfabetik sırayla getirir
    /// </summary>
    Task<IEnumerable<Site>> GetAllSitesOrderedAsync();
}

