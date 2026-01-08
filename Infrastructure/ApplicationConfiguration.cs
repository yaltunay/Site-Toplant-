using System.IO;
using Microsoft.Extensions.Configuration;

namespace Toplanti.Infrastructure;

/// <summary>
/// Uygulama konfigürasyon ayarlarını yönetir
/// </summary>
public class ApplicationConfiguration
{
    private readonly IConfiguration _configuration;

    public ApplicationConfiguration(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Database connection string'i döndürür
    /// </summary>
    public string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");
    }

    /// <summary>
    /// Hata dosyasının yolu
    /// </summary>
    public string GetErrorLogFilePath()
    {
        var fileName = _configuration["Application:ErrorLogFileName"] ?? "hata.txt";
        var customPath = _configuration["Application:ErrorLogPath"];
        
        if (!string.IsNullOrWhiteSpace(customPath))
        {
            return Path.Combine(customPath, fileName);
        }
        
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }

    /// <summary>
    /// Migration'ların etkin olup olmadığını kontrol eder
    /// </summary>
    public bool IsMigrationsEnabled()
    {
        return _configuration.GetValue<bool>("Database:EnableMigrations", true);
    }

    /// <summary>
    /// Seeding'in etkin olup olmadığını kontrol eder
    /// </summary>
    public bool IsSeedingEnabled()
    {
        return _configuration.GetValue<bool>("Database:EnableSeeding", true);
    }
}

