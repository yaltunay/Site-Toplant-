namespace Toplanti.Models.DTOs;

/// <summary>
/// Document entity için Data Transfer Object
/// </summary>
public class DocumentDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
}

