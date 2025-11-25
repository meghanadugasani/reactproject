using Sharemarketsimulation.DTOs;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> RegisterStaffAsync(RegisterStaffDto registerDto);
    }
}