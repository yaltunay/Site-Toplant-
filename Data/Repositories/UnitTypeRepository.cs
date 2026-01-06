using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// UnitType entity için repository implementation
/// </summary>
public class UnitTypeRepository : Repository<UnitType>, IUnitTypeRepository
{
    public UnitTypeRepository(ToplantiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UnitType>> GetAllUnitTypesAsync()
    {
        return await _dbSet.OrderBy(ut => ut.Name).ToListAsync();
    }
}

