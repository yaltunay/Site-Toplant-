using Microsoft.EntityFrameworkCore;
using Toplanti.Data;
using Toplanti.Models;

namespace Toplanti.Infrastructure;

/// <summary>
/// Database initialization ve seeding işlemlerini yönetir
/// </summary>
public class DatabaseInitializer
{
    private readonly ToplantiDbContext _context;

    public DatabaseInitializer(ToplantiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Database'i başlatır ve migration'ları uygular
    /// </summary>
    public async Task InitializeAsync()
    {
        // Database bağlantısını kontrol et
        if (_context.Database.CanConnect())
        {
            // Database mevcut, migration'ları uygula
            await _context.Database.MigrateAsync();
        }
        else
        {
            // Database yok, oluştur ve migration'ları uygula
            await _context.Database.MigrateAsync();
        }
    }

    /// <summary>
    /// Initial data seeding işlemlerini yapar
    /// </summary>
    public async Task SeedAsync()
    {
        // UnitTypes seed işlemi
        if (!await _context.UnitTypes.AnyAsync())
        {
            var unitTypes = new List<UnitType>
            {
                new UnitType 
                { 
                    Name = "Villa", 
                    LandShareMultiplier = 1.5m, 
                    Description = "Villa tipi birim" 
                },
                new UnitType 
                { 
                    Name = "Daire", 
                    LandShareMultiplier = 1.0m, 
                    Description = "Daire tipi birim" 
                },
                new UnitType 
                { 
                    Name = "Dükkan", 
                    LandShareMultiplier = 1.2m, 
                    Description = "Dükkan tipi birim" 
                }
            };

            await _context.UnitTypes.AddRangeAsync(unitTypes);
            await _context.SaveChangesAsync();
        }
    }
}

