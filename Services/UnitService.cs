using Toplanti.Data;
using Toplanti.Data.Repositories;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;
    private readonly ToplantiDbContext _context;

    public UnitService(IUnitRepository unitRepository, ToplantiDbContext context)
    {
        _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Unit>> GetUnitsBySiteIdAsync(int siteId)
    {
        return await _unitRepository.GetActiveUnitsBySiteIdAsync(siteId);
    }

    public async Task<bool> DeleteUnitAsync(int unitId)
    {
        var unit = await _unitRepository.GetByIdAsync(unitId);
        if (unit == null) return false;

        unit.IsActive = false;
        _unitRepository.Update(unit);
        await _context.SaveChangesAsync();
        return true;
    }
}

