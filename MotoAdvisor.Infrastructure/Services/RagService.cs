using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class RagService : IRagService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<RagService> _logger;
    private readonly Dictionary<int, float[]> _embeddings = new();
    private readonly Dictionary<int, MotorcycleInfo> _motorcycleInfos = new();
    private bool _initialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    private record MotorcycleInfo(
        int Id, string Name, string BrandName, string CategoryName,
        int Year, decimal Price, int Horsepower, string? MainImageUrl, string Description);

    public RagService(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<RagService> logger)
    {
        _scopeFactory = scopeFactory;
        _http = new HttpClient();
        _logger = logger;
        _apiKey = config["Gemini:ApiKey"]
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
            ?? throw new InvalidOperationException("Gemini API key not configured");
    }

    public async Task InitializeEmbeddingsAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_initialized && _embeddings.Count > 0)
            {
                _logger.LogDebug("Embeddings already initialized, skipping");
                return;
            }

            _logger.LogInformation("Initializing motorcycle embeddings...");
            _logger.LogInformation("Using Gemini API key: {KeyPrefix}...",
                _apiKey.Length > 10 ? _apiKey[..10] : "[too short]");

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var motorcycles = await db.Motorcycles
                .Include(m => m.Brand)
                .Include(m => m.Category)
                .Include(m => m.Images)
                .ToListAsync();

            _logger.LogInformation("Found {Count} motorcycles to embed", motorcycles.Count);

            foreach (var m in motorcycles)
            {
                var description = BuildDescription(m);
                var imageUrl = GetRealImageUrl(m);
                _logger.LogDebug("Motorcycle {Id} {Name}: {ImageCount} images, mainImageUrl={ImageUrl}",
                    m.Id, m.Name, m.Images.Count, imageUrl ?? "null");
                _motorcycleInfos[m.Id] = new MotorcycleInfo(
                    m.Id, m.Name, m.Brand.Name, m.Category.Name,
                    m.Year, m.Price, m.Horsepower,
                    imageUrl,
                    description);

                try
                {
                    var embedding = await GetEmbeddingAsync(description);
                    _embeddings[m.Id] = embedding;
                    _logger.LogDebug("Embedded motorcycle {Id}: {Name}", m.Id, m.Name);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to embed motorcycle {Id}: {Name}. Error: {Message}",
                        m.Id, m.Name, ex.Message);
                }
            }

            _initialized = true;
            _logger.LogInformation("Initialized {Count} motorcycle embeddings successfully", _embeddings.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize embeddings: {Message}", ex.Message);
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<RagRecommendationResult> RecommendAsync(string query)
    {
        if (_embeddings.Count == 0)
        {
            _logger.LogWarning("Embeddings not initialized, attempting on-demand initialization...");
            await InitializeEmbeddingsAsync();

            if (_embeddings.Count == 0)
                throw new InvalidOperationException("Failed to initialize embeddings - no motorcycles could be embedded");
        }

        float[] queryEmbedding;
        try
        {
            queryEmbedding = await GetEmbeddingAsync(query);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("429"))
        {
            _logger.LogWarning("Rate limited by Gemini API");
            return new RagRecommendationResult
            {
                AiResponse = "Sistemul este temporar suprasolicitat. Te rugam sa incerci din nou in cateva secunde.",
                Motorcycles = []
            };
        }

        var budgetLimit = ExtractBudget(query);

        var similarities = _embeddings
            .Select(kv => {
                var score = CosineSimilarity(queryEmbedding, kv.Value);
                if (budgetLimit.HasValue && _motorcycleInfos.TryGetValue(kv.Key, out var info))
                {
                    if (info.Price <= budgetLimit.Value)
                        score *= 1.15;
                    else
                        score *= 0.85;
                }
                return (Id: kv.Key, Score: score);
            })
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToList();

        var motorcycleIds = similarities.Select(s => s.Id).ToList();
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var motorcyclesWithImages = await db.Motorcycles
            .Include(m => m.Images)
            .Include(m => m.Brand)
            .Include(m => m.Category)
            .Where(m => motorcycleIds.Contains(m.Id))
            .ToListAsync();

        var topMotorcycles = similarities
            .Select(s => {
                var m = motorcyclesWithImages.First(x => x.Id == s.Id);
                return new RecommendedMotorcycle
                {
                    Id = m.Id,
                    Name = m.Name,
                    BrandName = m.Brand.Name,
                    CategoryName = m.Category.Name,
                    Year = m.Year,
                    Price = m.Price,
                    Horsepower = m.Horsepower,
                    MainImageUrl = GetRealImageUrl(m),
                    SimilarityScore = s.Score
                };
            })
            .ToList();

        var context = string.Join("\n\n", topMotorcycles.Select(m =>
            $"- {m.BrandName} {m.Name} ({m.Year}): {m.Horsepower} CP, pret {m.Price:N0}€, categorie {m.CategoryName}"));

        string aiResponse;
        try
        {
            aiResponse = await GenerateResponseAsync(query, context, budgetLimit);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("429"))
        {
            _logger.LogWarning("Rate limited by Gemini API during generation");
            aiResponse = "Sistemul de recomandare este temporar indisponibil din cauza limitelor API. Top 5 motociclete găsite sunt afișate mai jos bazat pe similaritate.";
        }

        return new RagRecommendationResult
        {
            AiResponse = aiResponse,
            Motorcycles = topMotorcycles
        };
    }

    private static string BuildDescription(Core.Entities.Motorcycle m)
    {
        var parts = new List<string>
        {
            $"{m.Brand.Name} {m.Name}",
            $"Anul {m.Year}",
            $"Categorie: {m.Category.Name}",
            $"Pret: {m.Price:N0} EUR"
        };

        if (m.Horsepower > 0) parts.Add($"Putere: {m.Horsepower} CP");
        if (!string.IsNullOrEmpty(m.Engine)) parts.Add($"Motor: {m.Engine}");
        if (!string.IsNullOrEmpty(m.Power)) parts.Add($"Putere: {m.Power}");
        if (!string.IsNullOrEmpty(m.LicenseCategory)) parts.Add($"Permis categoria {m.LicenseCategory}");
        if (m.IsBeginnerFriendly) parts.Add("Potrivita pentru incepatori");
        if (!string.IsNullOrEmpty(m.Description)) parts.Add(m.Description);

        return string.Join(". ", parts);
    }

    private async Task<float[]> GetEmbeddingAsync(string text)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:embedContent?key={_apiKey}";

        var requestBody = new
        {
            model = "models/gemini-embedding-001",
            content = new
            {
                parts = new[] { new { text = text } }
            }
        };

        var response = await _http.PostAsJsonAsync(url, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Gemini embedding API error {StatusCode}: {Error}", response.StatusCode, errorContent);
            throw new HttpRequestException($"Gemini API returned {response.StatusCode}: {errorContent}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var values = json.GetProperty("embedding").GetProperty("values");

        return values.EnumerateArray().Select(v => v.GetSingle()).ToArray();
    }

    private async Task<string> GenerateResponseAsync(string query, string context, decimal? budgetLimit)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

        var budgetNote = budgetLimit.HasValue
            ? $"\n\nBugetul clientului: {budgetLimit:N0}€. Daca unele motociclete depasesc acest buget, mentionează acest lucru și sugerează alternative mai accesibile din lista."
            : "";

        var prompt = $"""
            Esti un expert in motociclete care ajuta clientii sa gaseasca motocicleta ideala.
            Raspunde in limba romana, in mod prietenos si informativ.

            Clientul cauta: {query}{budgetNote}

            Bazat pe cautarea clientului, acestea sunt cele mai potrivite 5 motociclete din catalogul nostru:
            {context}

            Explica de ce aceste motociclete se potrivesc cerintelor clientului.
            Mentionezi scurt avantajele fiecareia si pentru cine este mai potrivita.
            Daca unele motociclete depasesc bugetul mentionat, spune clar acest lucru.
            Raspunsul trebuie sa fie concis (maxim 200 de cuvinte).
            """;

        var request = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { temperature = 0.7, maxOutputTokens = 500 }
        };

        var response = await _http.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";
    }

    private static decimal? ExtractBudget(string query)
    {
        var patterns = new[]
        {
            @"sub\s+(\d+(?:[\.,]\d+)?)\s*(?:euro|eur|€)?",
            @"buget\s+(?:de\s+)?(\d+(?:[\.,]\d+)?)\s*(?:euro|eur|€)?",
            @"max(?:im)?\s+(\d+(?:[\.,]\d+)?)\s*(?:euro|eur|€)?",
            @"pana\s+(?:la|in)\s+(\d+(?:[\.,]\d+)?)\s*(?:euro|eur|€)?",
            @"(\d+(?:[\.,]\d+)?)\s*(?:euro|eur|€)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(query, pattern, RegexOptions.IgnoreCase);
            if (match.Success && decimal.TryParse(match.Groups[1].Value.Replace(',', '.'), out var budget))
                return budget;
        }

        return null;
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        double dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }

    private static string? GetRealImageUrl(Core.Entities.Motorcycle m)
    {
        static bool IsReal(Core.Entities.MotorcycleImage i) => !i.ImageUrl.Contains("placehold.co");
        return m.Images.FirstOrDefault(i => i.IsMain && IsReal(i))?.ImageUrl
            ?? m.Images.FirstOrDefault(IsReal)?.ImageUrl;
    }
}
