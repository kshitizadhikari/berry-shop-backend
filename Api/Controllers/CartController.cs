using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Service.Services.IServices;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("UserId not found in token");

        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("UserId not found in token");

        var result = await _cartService.GetCartAsync(userId);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("UserId not found in token");

        await _cartService.ClearCartAsync(userId);
        return NoContent();
    }
}