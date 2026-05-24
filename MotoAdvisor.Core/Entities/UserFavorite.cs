namespace MotoAdvisor.Core.Entities;

public class UserFavorite
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int MotorcycleId { get; set; }
    public Motorcycle Motorcycle { get; set; } = null!;
}
