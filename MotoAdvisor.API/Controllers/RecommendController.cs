using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoAdvisor.Core.Interfaces;

namespace MotoAdvisor.API.Controllers;

[ApiController]
[Route("api/recommend")]
[Authorize]
public class RecommendController : ControllerBase
{
    private readonly IRagService _ragService;

    public RecommendController(IRagService ragService) => _ragService = ragService;

    [HttpPost]
    public async Task<IActionResult> Recommend([FromBody] RecommendRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { message = "Query cannot be empty" });

        var result = await _ragService.RecommendAsync(request.Query);
        return Ok(result);
    }
}

public record RecommendRequest(string Query);
