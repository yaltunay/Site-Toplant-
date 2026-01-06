namespace Toplanti.Models;

public class Unit
{
    public int Id { get; set; }
    public required string Number { get; set; }
    public required string OwnerName { get; set; } // Backward compatibility için korunuyor
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal LandShare { get; set; }
    public int UnitTypeId { get; set; }
    public UnitType? UnitType { get; set; }
    public string? Block { get; set; }
    public int? SiteId { get; set; }
    public Site? Site { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<MeetingAttendance> Attendances { get; set; } = [];
    public ICollection<Proxy> ProxiesGiven { get; set; } = [];
    public ICollection<Proxy> ProxiesReceived { get; set; } = [];
}

