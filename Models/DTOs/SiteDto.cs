namespace Toplanti.Models.DTOs;

/// <summary>
/// Site entity için Data Transfer Object
/// View katmanına sadece gerekli property'leri expose eder
/// </summary>
public class SiteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalLandShare { get; set; }
    public int UnitCount { get; set; }
}

