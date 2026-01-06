namespace Toplanti.Models;

public class Site
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal TotalLandShare { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
    
    public ICollection<Unit> Units { get; set; } = [];
}

