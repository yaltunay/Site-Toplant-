using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Site entity için repository implementation
/// </summary>
public class SiteRepository : Repository<Site>, ISiteRepository
{
    public SiteRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Site>> GetAllSitesOrderedAsync()
    {
        return await _dbSet
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}

