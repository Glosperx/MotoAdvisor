using System.ComponentModel.DataAnnotations;

namespace MotoAdvisor.Core.DTOs;

public record BrandDto(int Id, string Name, string? Country, string? LogoUrl);

public class CreateBrandDto
{
    [Required][MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? Country { get; set; }
    public string? LogoUrl { get; set; }
}

public class UpdateBrandDto
{
    [Required][MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? Country { get; set; }
    public string? LogoUrl { get; set; }
}
