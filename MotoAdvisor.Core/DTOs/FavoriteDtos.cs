namespace MotoAdvisor.Core.DTOs;

public record FavoriteDto(
    int MotorcycleId, string Name, int Year,
    decimal Price, string BrandName, string? MainImageUrl
);
