using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Dtos.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.IServices;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController(IOrderService orderService) : ControllerBase
{
    private string UserId => User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto request)
    {
        var order = await orderService.PlaceOrderAsync(UserId, request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
        => Ok(await orderService.GetMyOrdersAsync(UserId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await orderService.GetByIdAsync(UserId, id));

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
        => Ok(await orderService.GetAllAsync());
}