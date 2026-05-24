namespace MotoAdvisor.Core.Entities;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int MotorcycleId { get; set; }
    public Motorcycle Motorcycle { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}
