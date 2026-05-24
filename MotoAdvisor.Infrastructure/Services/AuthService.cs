using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;

namespace MotoAdvisor.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<(AuthResponseDto? Response, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (null, result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateToken(user, roles);
        return (new AuthResponseDto(token, user.Email!, user.UserName!, roles), []);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return null;

        if (!await _userManager.CheckPasswordAsync(user, dto.Password)) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateToken(user, roles);
        return new AuthResponseDto(token, user.Email!, user.UserName!, roles);
    }

    private string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(
            double.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
