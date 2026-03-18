using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Dtos.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.IServices;

namespace Api.Controllers;

[ApiController]
[Route("api/cart/items")]
[Authorize]
public class CartItemController(ICartItemService cartItemService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto request)
        => Ok(await cartItemService.AddItemAsync(UserId, request));

    [HttpPatch("{cartItemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid cartItemId, [FromBody] UpdateCartItemDto request)
        => Ok(await cartItemService.UpdateItemAsync(UserId, cartItemId, request));

    [HttpDelete("{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId)
        => Ok(await cartItemService.RemoveItemAsync(UserId, cartItemId));

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
        => Ok(await cartItemService.ClearCartAsync(UserId));
}