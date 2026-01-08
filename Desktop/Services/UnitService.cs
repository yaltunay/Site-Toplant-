using Toplanti.Data;
using Toplanti.Infrastructure.Mappings;
using Toplanti.Models.DTOs;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class UnitService : IUnitService
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IEnumerable<UnitDto>> GetUnitsBySiteIdAsync(int siteId)
    {
        var units = await _unitOfWork.Units.GetActiveUnitsBySiteIdAsync(siteId);
        return EntityMapper.ToDto(units);
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

