using Domain.Dtos;
using Domain.Dtos.Auth;

namespace Service.Services.IServices;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    // Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
}