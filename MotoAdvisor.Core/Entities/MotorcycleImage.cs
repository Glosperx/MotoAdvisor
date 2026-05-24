namespace MotoAdvisor.Core.Entities;

public class MotorcycleImage
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }

    public int MotorcycleId { get; set; }
    public Motorcycle Motorcycle { get; set; } = null!;
}
