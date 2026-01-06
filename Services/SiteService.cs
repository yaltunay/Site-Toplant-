using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class SiteService : ISiteService
{
    private readonly ToplantiDbContext _context;

    public SiteService(ToplantiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Site>> GetAllSitesAsync()
    {
        return await _context.Sites
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Site?> GetSiteByIdAsync(int siteId)
    {
        return await _context.Sites.FindAsync(siteId);
    }
}

