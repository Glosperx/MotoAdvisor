namespace MotoAdvisor.Core.Entities;

public class Motorcycle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string? Engine { get; set; }
    public string? Power { get; set; }
    public int Horsepower { get; set; }
    public string? LicenseCategory { get; set; }
    public bool IsBeginnerFriendly { get; set; }
    public string? Description { get; set; }
    public float[]? EmbeddingVector { get; set; }

    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<MotorcycleImage> Images { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<UserFavorite> FavoritedBy { get; set; } = [];
}
