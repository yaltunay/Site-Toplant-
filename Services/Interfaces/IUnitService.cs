using Toplanti.Models;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Birim işlemlerini yöneten servis arayüzü
/// </summary>
public interface IUnitService
{
    Task<IEnumerable<Unit>> GetUnitsBySiteIdAsync(int siteId);
    Task<bool> DeleteUnitAsync(int unitId);
}

