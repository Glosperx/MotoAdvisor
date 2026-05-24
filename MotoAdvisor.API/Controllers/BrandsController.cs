using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Interfaces;

namespace MotoAdvisor.API.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;

    public BrandsController(IBrandService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await _service.GetByIdAsync(id);
        return brand is null ? NotFound() : Ok(brand);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
    {
        var brand = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto dto)
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
