namespace Toplanti.Models;

public class MeetingAttendance
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public bool IsProxy { get; set; }
    public int? ProxyId { get; set; }
    public Proxy? Proxy { get; set; }
    public DateTime AttendedAt { get; set; } = DateTime.Now;
}

