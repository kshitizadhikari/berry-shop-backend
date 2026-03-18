using Domain.Dtos;
using Domain.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;
using Service.Services.IServices;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserDto request)
    {
        var res = await authService.RegisterAsync(request);
        return Ok(res);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var res = await authService.LoginAsync(request);
        return Ok(res);
    }
}