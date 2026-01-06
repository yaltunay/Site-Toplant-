namespace Toplanti.Models;

public class Meeting
{
    public int Id { get; set; }
    public required DateTime MeetingDate { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal TotalSiteLandShare { get; set; }
    public int TotalUnitCount { get; set; }
    public int AttendedUnitCount { get; set; }
    public decimal AttendedLandShare { get; set; }
    public bool QuorumAchieved { get; set; }
    public bool IsCompleted { get; set; } = false;
    public string? MeetingMinutes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<MeetingAttendance> Attendances { get; set; } = [];
    public ICollection<Vote> Votes { get; set; } = [];
    public ICollection<Decision> Decisions { get; set; } = [];
    public ICollection<Proxy> Proxies { get; set; } = [];
    public ICollection<AgendaItem> AgendaItems { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
}

