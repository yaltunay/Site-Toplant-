namespace Toplanti.Models.DTOs;

/// <summary>
/// Meeting entity için Data Transfer Object
/// View katmanına sadece gerekli property'leri expose eder
/// </summary>
public class MeetingDto
{
    public int Id { get; set; }
    public DateTime MeetingDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalSiteLandShare { get; set; }
    public int TotalUnitCount { get; set; }
    public int AttendedUnitCount { get; set; }
    public decimal AttendedLandShare { get; set; }
    public bool QuorumAchieved { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties - sadece ID'ler veya basit bilgiler
    public int AgendaItemCount { get; set; }
    public int DocumentCount { get; set; }
    public int DecisionCount { get; set; }
}

