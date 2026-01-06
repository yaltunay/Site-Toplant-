namespace Toplanti.Services.Interfaces;

/// <summary>
/// Yeter sayı kontrolü yapan servis arayüzü
/// </summary>
public interface IQuorumService
{
    (bool achieved, string message) CheckQuorum(
        int totalUnitCount,
        int attendedUnitCount,
        decimal totalLandShare,
        decimal attendedLandShare);
    
    string GenerateQuorumNote(decimal totalLandShare, decimal attendedLandShare);
}

