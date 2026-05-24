namespace MotoAdvisor.Core.Entities;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? LogoUrl { get; set; }

    public ICollection<Motorcycle> Motorcycles { get; set; } = [];
}
