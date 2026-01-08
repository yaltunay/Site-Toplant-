using Toplanti.Models;
using Toplanti.Models.DTOs;

namespace Toplanti.Services.Interfaces;

/// <summary>
/// Toplantı işlemlerini yöneten servis arayüzü
/// </summary>
public interface IMeetingService
{
    Task<MeetingDto> CreateMeetingAsync(string title, DateTime meetingDate, Site site);
    Task<bool> CompleteMeetingAsync(int meetingId);
    Task<QuorumResult> CheckQuorumAsync(int meetingId);
    Task<string> GenerateMeetingMinutesAsync(int meetingId);
    Task<IEnumerable<MeetingDto>> GetMeetingsBySiteIdAsync(int siteId);
    Task<MeetingDto?> GetMeetingByIdAsync(int meetingId);
    Task<DashboardStats> GetDashboardStatsAsync(int siteId);
    
    // Domain model döndüren metodlar (iç işlemler için gerekli)
    Task<Meeting?> GetMeetingDomainModelByIdAsync(int meetingId);
}

public class QuorumResult
{
    public bool Achieved { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DashboardStats
{
    public int TotalUnits { get; set; }
    public int TotalMeetings { get; set; }
    public int TotalDecisions { get; set; }
    public decimal TotalLandShare { get; set; }
}

