namespace Toplanti.Models;

public class Decision
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int YesVotes { get; set; }
    public int NoVotes { get; set; }
    public int AbstainVotes { get; set; }
    public decimal YesLandShare { get; set; }
    public decimal NoLandShare { get; set; }
    public decimal AbstainLandShare { get; set; }
    public bool IsApproved { get; set; }
    public string? DecisionText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<Vote> Votes { get; set; } = [];
}

