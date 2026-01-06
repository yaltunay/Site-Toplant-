using Toplanti.Models;

namespace Toplanti.Data.Repositories;

/// <summary>
/// UnitType entity için repository interface
/// </summary>
public interface IUnitTypeRepository : IRepository<UnitType>
{
    /// <summary>
    /// Tüm birim tiplerini getirir
    /// </summary>
    Task<IEnumerable<UnitType>> GetAllUnitTypesAsync();
}

