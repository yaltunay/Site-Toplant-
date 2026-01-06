using Toplanti.Models;
using Toplanti.Models.DTOs;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Karar/Oylama işlemlerini yöneten servis arayüzü
/// </summary>
public interface IDecisionService
{
    Task<DecisionDto> CreateDecisionAsync(int meetingId, string title, string description);
    Task<IEnumerable<DecisionDto>> GetDecisionsByMeetingIdAsync(int meetingId);
    Task<bool> DeleteDecisionAsync(int decisionId);
    
    // Domain model döndüren metod (detay görüntüleme için gerekli)
    Task<Decision?> GetDecisionDomainModelByIdAsync(int decisionId);
}

