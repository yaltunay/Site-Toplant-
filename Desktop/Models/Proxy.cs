namespace Toplanti.Models;

public class Proxy
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public int GiverUnitId { get; set; }
    public Unit? GiverUnit { get; set; }
    public int? ReceiverUnitId { get; set; }
    public Unit? ReceiverUnit { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsUsed { get; set; } = false;
}

