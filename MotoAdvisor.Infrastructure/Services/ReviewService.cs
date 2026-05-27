using Microsoft.Extensions.Logging;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repo;
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IReviewRepository repo, AppDbContext context, ILogger<ReviewService> logger)
    {
        _repo = repo;
        _context = context;
        _logger = logger;
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
        _logger.LogInformation("Review created with id {Id} for motorcycle {MotorcycleId} by user {UserId}", review.Id, motorcycleId, userId);
        return new ReviewDto(review.Id, review.Rating, review.Content, review.CreatedAt, userName, userId);
    }

    public async Task<bool> UpdateAsync(int id, string userId, UpdateReviewDto dto)
    {
        var review = await _repo.GetByIdAsync(id);
        if (review is null || review.UserId != userId)
        {
            _logger.LogWarning("Update failed — review with id {Id} not found or user {UserId} is not the owner", id, userId);
            return false;
        }

        review.Rating = dto.Rating;
        review.Content = dto.Content;
        await _repo.UpdateAsync(review);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Review with id {Id} updated by user {UserId}", id, userId);
        return true;
    }

    public async Task<ReviewDeleteResult> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var review = await _repo.GetByIdAsync(id);
        if (review is null)
        {
            _logger.LogWarning("Delete failed — review with id {Id} not found", id);
            return ReviewDeleteResult.NotFound;
        }
        if (!isAdmin && review.UserId != userId)
        {
            _logger.LogWarning("Delete forbidden — user {UserId} attempted to delete review {Id} owned by {OwnerId}", userId, id, review.UserId);
            return ReviewDeleteResult.Forbidden;
        }

        await _repo.DeleteAsync(review);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Review with id {Id} deleted by user {UserId} (admin: {IsAdmin})", id, userId, isAdmin);
        return ReviewDeleteResult.Success;
    }

    private static ReviewDto Map(Review r) => new(
        r.Id, r.Rating, r.Content, r.CreatedAt,
        r.User?.UserName ?? string.Empty, r.UserId
    );
}
