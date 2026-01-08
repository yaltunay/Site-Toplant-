namespace Toplanti.Models;

public class AgendaItem
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

