using Microsoft.AspNetCore.Mvc;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Interfaces;

namespace MotoAdvisor.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (response, errors) = await _auth.RegisterAsync(dto);
        if (response is null)
            return BadRequest(new { errors });

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await _auth.LoginAsync(dto);
        if (response is null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(response);
    }
}
