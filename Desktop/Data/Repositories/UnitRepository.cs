using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// Unit entity için repository implementation
/// </summary>
public class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Unit>> GetUnitsBySiteIdAsync(int siteId)
    {
        return await _dbSet
            .Include(u => u.UnitType)
            .Where(u => u.SiteId == siteId)
            .OrderBy(u => u.Block)
            .ThenBy(u => u.Number)
            .ToListAsync();
    }

    public async Task<IEnumerable<Unit>> GetActiveUnitsBySiteIdAsync(int siteId)
    {
        return await _dbSet
            .Include(u => u.UnitType)
            .Where(u => u.SiteId == siteId && u.IsActive)
            .OrderBy(u => u.Block)
            .ThenBy(u => u.Number)
            .ToListAsync();
    }

    public async Task<int> GetActiveUnitCountBySiteIdAsync(int siteId)
    {
        return await _dbSet.CountAsync(u => u.SiteId == siteId && u.IsActive);
    }
}

