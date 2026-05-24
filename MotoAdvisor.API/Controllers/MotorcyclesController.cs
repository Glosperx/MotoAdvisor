using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Interfaces;

namespace MotoAdvisor.API.Controllers;

[ApiController]
[Route("api/motorcycles")]
public class MotorcyclesController : ControllerBase
{
    private readonly IMotorcycleService _service;

    public MotorcyclesController(IMotorcycleService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? brandId,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice) =>
        Ok(await _service.GetAllAsync(brandId, categoryId, minPrice, maxPrice));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var motorcycle = await _service.GetByIdAsync(id);
        return motorcycle is null ? NotFound() : Ok(motorcycle);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search query cannot be empty." });

        return Ok(await _service.SearchAsync(q));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto dto)
    {
        var motorcycle = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = motorcycle.Id }, motorcycle);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMotorcycleDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
