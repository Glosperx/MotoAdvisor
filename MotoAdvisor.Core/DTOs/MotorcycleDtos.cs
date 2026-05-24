using System.ComponentModel.DataAnnotations;

namespace MotoAdvisor.Core.DTOs;

public record MotorcycleSummaryDto(
    int Id, string Name, int Year, decimal Price,
    string? Engine, string? Power, int Horsepower,
    string? LicenseCategory, bool IsBeginnerFriendly,
    int BrandId, string BrandName,
    int CategoryId, string CategoryName,
    string? MainImageUrl
);

public record MotorcycleDetailDto(
    int Id, string Name, int Year, decimal Price,
    string? Engine, string? Power, int Horsepower,
    string? LicenseCategory, bool IsBeginnerFriendly,
    string? Description,
    BrandDto Brand, CategoryDto Category,
    List<string> ImageUrls,
    double? AverageRating, int ReviewCount
);

public class CreateMotorcycleDto
{
    [Required][MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Range(1900, 2100)]
    public int Year { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    public string? Engine { get; set; }
    public string? Power { get; set; }
    public int Horsepower { get; set; }
    public string? LicenseCategory { get; set; }
    public bool IsBeginnerFriendly { get; set; }
    public string? Description { get; set; }
    [Required]
    public int BrandId { get; set; }
    [Required]
    public int CategoryId { get; set; }
}

public class UpdateMotorcycleDto
{
    [Required][MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Range(1900, 2100)]
    public int Year { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    public string? Engine { get; set; }
    public string? Power { get; set; }
    public int Horsepower { get; set; }
    public string? LicenseCategory { get; set; }
    public bool IsBeginnerFriendly { get; set; }
    public string? Description { get; set; }
    [Required]
    public int BrandId { get; set; }
    [Required]
    public int CategoryId { get; set; }
}
