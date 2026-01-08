namespace Toplanti.Models;

public class Vote
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public int DecisionId { get; set; }
    public Decision? Decision { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public VoteType VoteType { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.Now;
}

public enum VoteType
{
    Yes = 1,
    No = 2,
    Abstain = 3
}

