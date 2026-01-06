using Toplanti.Data.Repositories;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class SiteService : ISiteService
{
    private readonly ISiteRepository _siteRepository;

    public SiteService(ISiteRepository siteRepository)
    {
        _siteRepository = siteRepository ?? throw new ArgumentNullException(nameof(siteRepository));
    }

    public async Task<IEnumerable<Site>> GetAllSitesAsync()
    {
        return await _siteRepository.GetAllSitesOrderedAsync();
    }

    public async Task<Site?> GetSiteByIdAsync(int siteId)
    {
        return await _siteRepository.GetByIdAsync(siteId);
    }
}

