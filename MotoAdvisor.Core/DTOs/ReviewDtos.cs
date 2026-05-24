using System.ComponentModel.DataAnnotations;

namespace MotoAdvisor.Core.DTOs;

public record ReviewDto(
    int Id, int Rating, string? Content,
    DateTime CreatedAt, string UserName, string UserId
);

public class CreateReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }
    [MaxLength(2000)]
    public string? Content { get; set; }
}

public class UpdateReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }
    [MaxLength(2000)]
    public string? Content { get; set; }
}

public enum ReviewDeleteResult { Success, NotFound, Forbidden }
