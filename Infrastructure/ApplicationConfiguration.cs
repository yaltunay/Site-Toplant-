namespace Toplanti.Infrastructure;

/// <summary>
/// Uygulama konfigürasyon ayarlarını yönetir
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Database connection string'i döndürür
    /// </summary>
    public static string GetConnectionString()
    {
        // Şimdilik hardcoded, ileride appsettings.json'dan okunabilir
        return "Server=(localdb)\\mssqllocaldb;Database=ToplantiDb;Trusted_Connection=true;MultipleActiveResultSets=true";
    }

    /// <summary>
    /// Hata dosyasının yolu
    /// </summary>
    public static string GetErrorLogFilePath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hata.txt");
    }
}

