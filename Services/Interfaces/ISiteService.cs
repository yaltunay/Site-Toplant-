using Toplanti.Models;
using Toplanti.Models.DTOs;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Site işlemlerini yöneten servis arayüzü
/// </summary>
public interface ISiteService
{
    Task<IEnumerable<SiteDto>> GetAllSitesAsync();
    Task<SiteDto?> GetSiteByIdAsync(int siteId);
    
    // Domain model döndüren metodlar (backward compatibility için)
    Task<Site?> GetSiteDomainModelByIdAsync(int siteId);
}

