namespace Toplanti.Models.DTOs;

/// <summary>
/// Unit entity için Data Transfer Object
/// View katmanına sadece gerekli property'leri expose eder
/// </summary>
public class UnitDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal LandShare { get; set; }
    public int UnitTypeId { get; set; }
    public string? UnitTypeName { get; set; }
    public string? Block { get; set; }
    public int? SiteId { get; set; }
    public bool IsActive { get; set; }
}

