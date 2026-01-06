using Toplanti.Models.DTOs;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Birim işlemlerini yöneten servis arayüzü
/// </summary>
public interface IUnitService
{
    Task<IEnumerable<UnitDto>> GetUnitsBySiteIdAsync(int siteId);
    Task<bool> DeleteUnitAsync(int unitId);
}

