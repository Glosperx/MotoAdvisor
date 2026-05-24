using System.ComponentModel.DataAnnotations;

namespace MotoAdvisor.Core.DTOs;

public class RegisterDto
{
    [Required][MaxLength(50)]
    public string UserName { get; set; } = string.Empty;
    [Required][EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required][MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    [Required][EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public record AuthResponseDto(string Token, string Email, string UserName, IList<string> Roles);
