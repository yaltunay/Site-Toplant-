namespace Toplanti.Models.DTOs;

/// <summary>
/// Decision entity için Data Transfer Object
/// View katmanına sadece gerekli property'leri expose eder
/// </summary>
public class DecisionDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public string MeetingTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int YesVotes { get; set; }
    public int NoVotes { get; set; }
    public int AbstainVotes { get; set; }
    public decimal YesLandShare { get; set; }
    public decimal NoLandShare { get; set; }
    public decimal AbstainLandShare { get; set; }
    public bool IsApproved { get; set; }
    public string? DecisionText { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Vote count summary
    public int TotalVotes { get; set; }
}

