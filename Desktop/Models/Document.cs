namespace Toplanti.Models;

public class Document
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public required string Title { get; set; }
    public required string DocumentType { get; set; } // "Genel Kurul İcraat Raporu", "Denetçi Raporu", "Diğer"
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

