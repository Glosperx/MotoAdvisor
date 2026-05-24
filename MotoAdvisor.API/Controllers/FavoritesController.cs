using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoAdvisor.Core.Interfaces;

namespace MotoAdvisor.API.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _service;

    public FavoritesController(IFavoriteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _service.GetUserFavoritesAsync(userId));
    }

    [HttpPost("{motorcycleId:int}")]
    public async Task<IActionResult> Add(int motorcycleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var added = await _service.AddFavoriteAsync(userId, motorcycleId);

        if (!added)
            return Conflict(new { message = "Motorcycle not found or already in favorites." });

        return StatusCode(201);
    }

    [HttpDelete("{motorcycleId:int}")]
    public async Task<IActionResult> Remove(int motorcycleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var removed = await _service.RemoveFavoriteAsync(userId, motorcycleId);
        return removed ? NoContent() : NotFound();
    }
}
