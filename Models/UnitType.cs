namespace Toplanti.Models;

public class UnitType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal LandShareMultiplier { get; set; }
    public string? Description { get; set; }
    
    public ICollection<Unit> Units { get; set; } = [];
}

