using Toplanti.Data.Repositories;
using Toplanti.Infrastructure.Mappings;
using Toplanti.Models;
using Toplanti.Models.DTOs;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class SiteService : ISiteService
{
    private readonly ISiteRepository _siteRepository;

    public SiteService(ISiteRepository siteRepository)
    {
        _siteRepository = siteRepository ?? throw new ArgumentNullException(nameof(siteRepository));
    }

    public async Task<IEnumerable<SiteDto>> GetAllSitesAsync()
    {
        var sites = await _siteRepository.GetAllSitesOrderedAsync();
        return EntityMapper.ToDto(sites);
    }

    public async Task<SiteDto?> GetSiteByIdAsync(int siteId)
    {
        var site = await _siteRepository.GetByIdAsync(siteId);
        return site != null ? EntityMapper.ToDto(site) : null;
    }

    public async Task<Site?> GetSiteDomainModelByIdAsync(int siteId)
    {
        return await _siteRepository.GetByIdAsync(siteId);
    }
}

