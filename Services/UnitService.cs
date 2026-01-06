using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class UnitService : IUnitService
{
    private readonly ToplantiDbContext _context;

    public UnitService(ToplantiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Unit>> GetUnitsBySiteIdAsync(int siteId)
    {
        return await _context.Units
            .Include(u => u.UnitType)
            .Where(u => u.SiteId == siteId && u.IsActive)
            .OrderBy(u => u.Block)
            .ThenBy(u => u.Number)
            .ToListAsync();
    }

    public async Task<bool> DeleteUnitAsync(int unitId)
    {
        var unit = await _context.Units.FindAsync(unitId);
        if (unit == null) return false;

        unit.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}

