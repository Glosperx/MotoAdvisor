using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repo;
    private readonly AppDbContext _context;

    public ReviewService(IReviewRepository repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<ReviewDto>> GetByMotorcycleAsync(int motorcycleId) =>
        (await _repo.GetByMotorcycleIdAsync(motorcycleId)).Select(Map);

    public async Task<ReviewDto> CreateAsync(int motorcycleId, string userId, string userName, CreateReviewDto dto)
    {
        var review = new Review
        {
            MotorcycleId = motorcycleId,
            UserId = userId,
            Rating = dto.Rating,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };
        await _repo.AddAsync(review);
        await _context.SaveChangesAsync();
        return new ReviewDto(review.Id, review.Rating, review.Content, review.CreatedAt, userName, userId);
    }

    public async Task<bool> UpdateAsync(int id, string userId, UpdateReviewDto dto)
    {
        var review = await _repo.GetByIdAsync(id);
        if (review is null || review.UserId != userId) return false;

        review.Rating = dto.Rating;
        review.Content = dto.Content;
        await _repo.UpdateAsync(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ReviewDeleteResult> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var review = await _repo.GetByIdAsync(id);
        if (review is null) return ReviewDeleteResult.NotFound;
        if (!isAdmin && review.UserId != userId) return ReviewDeleteResult.Forbidden;

        await _repo.DeleteAsync(review);
        await _context.SaveChangesAsync();
        return ReviewDeleteResult.Success;
    }

    private static ReviewDto Map(Review r) => new(
        r.Id, r.Rating, r.Content, r.CreatedAt,
        r.User?.UserName ?? string.Empty, r.UserId
    );
}
