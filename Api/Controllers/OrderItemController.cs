using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.IServices;

namespace Api.Controllers;

[ApiController]
[Route("api/orders/{orderId:guid}/items")]
[Authorize]
public class OrderItemController(IOrderItemService orderItemService) :  ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetByOrderId(Guid orderId)
        => Ok(await orderItemService.GetByOrderIdAsync(UserId, orderId));

    [HttpGet("{itemId:guid}")]
    public async Task<IActionResult> GetById(Guid orderId, Guid itemId)
        => Ok(await orderItemService.GetByIdAsync(UserId, itemId));

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
        => Ok(await orderItemService.GetAllAsync());
}