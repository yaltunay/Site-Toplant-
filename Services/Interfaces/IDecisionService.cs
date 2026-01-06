using Toplanti.Models;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Karar/Oylama işlemlerini yöneten servis arayüzü
/// </summary>
public interface IDecisionService
{
    Task<Decision> CreateDecisionAsync(int meetingId, string title, string description);
    Task<IEnumerable<Decision>> GetDecisionsByMeetingIdAsync(int meetingId);
    Task<bool> DeleteDecisionAsync(int decisionId);
}

