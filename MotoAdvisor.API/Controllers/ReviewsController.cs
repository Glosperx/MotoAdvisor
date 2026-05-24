using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Interfaces;
using static MotoAdvisor.Core.DTOs.ReviewDeleteResult;

namespace MotoAdvisor.API.Controllers;

[ApiController]
[Route("api")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewsController(IReviewService service) => _service = service;

    [HttpGet("motorcycles/{motorcycleId:int}/reviews")]
    public async Task<IActionResult> GetByMotorcycle(int motorcycleId) =>
        Ok(await _service.GetByMotorcycleAsync(motorcycleId));

    [HttpPost("motorcycles/{motorcycleId:int}/reviews")]
    [Authorize]
    public async Task<IActionResult> Create(int motorcycleId, [FromBody] CreateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userName = User.FindFirstValue(ClaimTypes.Name)!;

        var review = await _service.CreateAsync(motorcycleId, userId, userName, dto);
        return CreatedAtAction(nameof(GetByMotorcycle), new { motorcycleId }, review);
    }

    [HttpPut("reviews/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var updated = await _service.UpdateAsync(id, userId, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("reviews/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        return await _service.DeleteAsync(id, userId, isAdmin) switch
        {
            Success   => NoContent(),
            Forbidden => Forbid(),
            _         => NotFound()
        };
    }
}
