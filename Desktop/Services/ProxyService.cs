using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Services;

public class ProxyService : IProxyService
{
    public int CalculateMaxProxyCount(int totalUnitCount)
    {
        // KMK 31: If units > 40, max proxy = 5% of total units (rounded down). Else, max 2.
        if (totalUnitCount > 40)
        {
            return (int)Math.Floor(totalUnitCount * 0.05m);
        }
        return 2;
    }

    public decimal CalculateMaxProxyLandShare(decimal totalLandShare)
    {
        // KMK 31: Maximum proxy land share = 5% of total land share
        return totalLandShare * 0.05m;
    }

    public (bool isValid, string message) ValidateProxyCount(int currentProxyCount, int totalUnitCount)
    {
        var maxProxies = CalculateMaxProxyCount(totalUnitCount);
        
        if (currentProxyCount >= maxProxies)
        {
            return (false, $"Vekalet sayısı limiti aşıldı. Maksimum vekalet sayısı: {maxProxies} (KMK 31)");
        }
        
        return (true, $"Mevcut vekalet sayısı: {currentProxyCount}/{maxProxies}");
    }

    public (bool isValid, string message) ValidateProxyLimits(
        int currentProxyCount, 
        decimal currentProxyLandShare,
        int selectedProxyCount,
        decimal selectedProxyLandShare,
        int totalUnitCount,
        decimal totalLandShare)
    {
        var maxProxies = CalculateMaxProxyCount(totalUnitCount);
        var maxProxyLandShare = CalculateMaxProxyLandShare(totalLandShare);
        
        var newTotalProxyCount = currentProxyCount + selectedProxyCount;
        var newTotalProxyLandShare = currentProxyLandShare + selectedProxyLandShare;
        
        var countExceeded = newTotalProxyCount > maxProxies;
        var landShareExceeded = newTotalProxyLandShare > maxProxyLandShare;
        
        if (countExceeded && landShareExceeded)
        {
            return (false, $"Yasal limitler asildi!\n\n" +
                          $"Sayi limiti: {newTotalProxyCount}/{maxProxies} (KMK 31)\n" +
                          $"Arsa payi limiti: {newTotalProxyLandShare:F2}/{maxProxyLandShare:F2} (KMK 31)");
        }
        else if (countExceeded)
        {
            return (false, $"Sayi limiti asildi! {newTotalProxyCount}/{maxProxies} (KMK 31)");
        }
        else if (landShareExceeded)
        {
            return (false, $"Arsa payi limiti asildi! {newTotalProxyLandShare:F2}/{maxProxyLandShare:F2} (KMK 31)");
        }
        
        return (true, $"Sayi: {newTotalProxyCount}/{maxProxies}, Arsa Payi: {newTotalProxyLandShare:F2}/{maxProxyLandShare:F2} (KMK 31)");
    }
}

