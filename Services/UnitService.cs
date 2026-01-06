using Toplanti.Data;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class UnitService : IUnitService
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IEnumerable<Unit>> GetUnitsBySiteIdAsync(int siteId)
    {
        return await _unitOfWork.Units.GetActiveUnitsBySiteIdAsync(siteId);
    }

    public async Task<bool> DeleteUnitAsync(int unitId)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(unitId);
        if (unit == null) return false;

        unit.IsActive = false;
        _unitOfWork.Units.Update(unit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

