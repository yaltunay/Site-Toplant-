namespace Toplanti.Models.DTOs;

/// <summary>
/// AgendaItem entity için Data Transfer Object
/// </summary>
public class AgendaItemDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
}

