using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Dtos;
using Domain.Dtos.Auth;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Services.IServices;

namespace Service.Services;

public class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IConfiguration config)
    : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager = signInManager;

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto request)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Customer");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = GenerateJwt(user, roles);

        var userDto = new UserDto
        {
            Id = user.Id,
            Address = user.Address,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };

        return new AuthResponseDto
        {
            access_token = accessToken,
            token_type = "Bearer",
            expires_in = 3600,
            user = userDto
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
                   ?? throw new UnauthorizedException("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            throw new UnauthorizedException("Account is locked out. Try again later.");

        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = GenerateJwt(user, roles);

        var userDto = new UserDto
        {
            Id = user.Id,
            Address = user.Address,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };

        return new AuthResponseDto
        {
            access_token = accessToken,
            token_type = "Bearer",
            expires_in = 3600,
            user = userDto
        };
    }

    // public async Task ChangePasswordAsync(string userId, ChangePasswordRequest request)
    // {
    //     var user = await _userManager.FindByIdAsync(userId)
    //                ?? throw new NotFoundException("User", userId);
    //
    //     var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
    //     if (!result.Succeeded)
    //         throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));
    // }

    private string GenerateJwt(AppUser user, IList<string> roles)
    {
        var jwtSection = config.GetSection("Jwt");

        var keyString = jwtSection["Key"] ??
                        throw new InvalidOperationException("Jwt:Key is missing from configuration.");
        var issuer = jwtSection["Issuer"] ??
                     throw new InvalidOperationException("Jwt:Issuer is missing from configuration.");
        var audience = jwtSection["Audience"] ??
                       throw new InvalidOperationException("Jwt:Audience is missing from configuration.");
        var expiresInHours = jwtSection["ExpiresInHours"] ??
                             throw new InvalidOperationException("Jwt:ExpiresInHours is missing from configuration.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName ?? ""),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(expiresInHours)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}