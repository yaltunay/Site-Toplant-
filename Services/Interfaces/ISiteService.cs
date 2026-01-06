using Toplanti.Models;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Site işlemlerini yöneten servis arayüzü
/// </summary>
public interface ISiteService
{
    Task<IEnumerable<Site>> GetAllSitesAsync();
    Task<Site?> GetSiteByIdAsync(int siteId);
}

