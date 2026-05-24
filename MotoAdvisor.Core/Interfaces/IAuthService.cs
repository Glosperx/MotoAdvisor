using MotoAdvisor.Core.DTOs;

namespace MotoAdvisor.Core.Interfaces;

public interface IAuthService
{
    Task<(AuthResponseDto? Response, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}
