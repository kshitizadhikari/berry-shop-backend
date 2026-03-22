using System.Security.Claims;
using Domain.Dtos.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Service.Services.IServices;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartItemController(ICartItemService cartItemService) : ControllerBase
{
    private string UserId => User.FindFirstValue(JwtRegisteredClaimNames.Sub);


    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto request)
    {
        if (UserId is null)
            return Unauthorized();

        return Ok(await cartItemService.AddItemAsync(UserId, request));
    }

    [HttpPatch("{cartItemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid cartItemId, [FromBody] UpdateCartItemDto request)
        => Ok(await cartItemService.UpdateItemAsync(UserId, cartItemId, request));

    [HttpDelete("{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId)
        => Ok(await cartItemService.RemoveItemAsync(UserId, cartItemId));

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
        => Ok(await cartItemService.ClearCartAsync(UserId));

    [HttpGet("debug")]
    public IActionResult DebugClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}