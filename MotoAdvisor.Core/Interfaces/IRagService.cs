namespace MotoAdvisor.Core.Interfaces;

public interface IRagService
{
    Task InitializeEmbeddingsAsync();
    Task<RagRecommendationResult> RecommendAsync(string query);
}

public class RagRecommendationResult
{
    public string AiResponse { get; set; } = string.Empty;
    public List<RecommendedMotorcycle> Motorcycles { get; set; } = [];
}

public class RecommendedMotorcycle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public int Horsepower { get; set; }
    public string? MainImageUrl { get; set; }
    public string? CategoryName { get; set; }
    public double SimilarityScore { get; set; }
}
