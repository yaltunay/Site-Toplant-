namespace Toplanti.Services.Interfaces;

/// <summary>
/// Vekalet işlemlerini yöneten servis arayüzü
/// </summary>
public interface IProxyService
{
    int CalculateMaxProxyCount(int totalUnitCount);
    decimal CalculateMaxProxyLandShare(decimal totalLandShare);
    (bool isValid, string message) ValidateProxyCount(int currentProxyCount, int totalUnitCount);
    (bool isValid, string message) ValidateProxyLimits(
        int currentProxyCount,
        decimal currentProxyLandShare,
        int selectedProxyCount,
        decimal selectedProxyLandShare,
        int totalUnitCount,
        decimal totalLandShare);
}

