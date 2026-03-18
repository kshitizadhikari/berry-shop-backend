using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Service.Services.IServices;

namespace Service.Services;

public class OrderService : IOrderService
{
    // private readonly IUnitOfWork _uow;
    //
    // public OrderService(IUnitOfWork uow) => _uow = uow;
    //
    // public async Task<OrderResponse> PlaceOrderAsync(string userId, PlaceOrderRequest request)
    // {
    //     var cart = await _uow.Carts.Query()
    //         .Include(c => c.Items.Where(i => !i.IsDeleted))
    //         .ThenInclude(i => i.Product)
    //         .FirstOrDefaultAsync(c => c.UserId == userId);
    //
    //     if (cart is null || !cart.Items.Any())
    //         throw new BadRequestException("Your cart is empty.");
    //
    //     await _uow.BeginTransactionAsync();
    //
    //     foreach (var item in cart.Items)
    //     {
    //         if (!item.Product.IsActive)
    //             throw new BadRequestException($"Product '{item.Product.Name}' is no longer available.");
    //
    //         if (item.Product.StockQuantity < item.Quantity)
    //             throw new BadRequestException(
    //                 $"Insufficient stock for '{item.Product.Name}'. Available: {item.Product.StockQuantity}.");
    //
    //         item.Product.StockQuantity -= item.Quantity;
    //         item.Product.UpdatedAt = DateTime.UtcNow;
    //         _uow.Products.Update(item.Product);
    //     }
    //
    //     var order = new Order
    //     {
    //         UserId = userId,
    //         ShippingAddress = request.ShippingAddress,
    //         TotalAmount = cart.Items.Sum(i => i.Product.Price * i.Quantity),
    //         CreatedAt = DateTime.UtcNow,
    //         Items = cart.Items.Select(i => new OrderItem
    //         {
    //             ProductId = i.ProductId,
    //             Quantity = i.Quantity,
    //             UnitPrice = i.Product.Price,
    //             CreatedAt = DateTime.UtcNow
    //         }).ToList()
    //     };
    //
    //     await _uow.Orders.AddAsync(order);
    //
    //     foreach (var item in cart.Items)
    //     {
    //         item.IsDeleted = true;
    //         item.DeletedAt = DateTime.UtcNow;
    //         _uow.CartItems.Update(item);
    //     }
    //
    //     await _uow.CommitTransactionAsync();
    //
    //     return ToResponse(order);
    // }
    //
    // public async Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId)
    // {
    //     var orders = await _uow.Orders.Query()
    //         .Include(o => o.Items)
    //         .ThenInclude(i => i.Product)
    //         .Where(o => o.UserId == userId)
    //         .OrderByDescending(o => o.CreatedAt)
    //         .ToListAsync();
    //
    //     return orders.Select(ToResponse);
    // }
    //
    // public async Task<OrderResponse> GetOrderByIdAsync(string userId, int orderId)
    // {
    //     var order = await _uow.Orders.Query()
    //                     .Include(o => o.Items)
    //                     .ThenInclude(i => i.Product)
    //                     .FirstOrDefaultAsync(o => o.Id == new Guid(orderId.ToString()))
    //                 ?? throw new NotFoundException("Order", orderId);
    //
    //     if (order.UserId != userId)
    //         throw new UnauthorizedException("You do not have access to this order.");
    //
    //     return ToResponse(order);
    // }
    //
    // public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    // {
    //     var orders = await _uow.Orders.Query()
    //         .Include(o => o.Items)
    //         .ThenInclude(i => i.Product)
    //         .OrderByDescending(o => o.CreatedAt)
    //         .ToListAsync();
    //
    //     return orders.Select(ToResponse);
    // }
    //
    // private static OrderResponse ToResponse(Order o) => new(
    //     o.Id,
    //     o.Status,
    //     o.ShippingAddress,
    //     o.TotalAmount,
    //     o.CreatedAt,
    //     o.Items.Select(i => new OrderItemResponse(
    //         i.ProductId,
    //         i.Product?.Name ?? string.Empty,
    //         i.Quantity,
    //         i.UnitPrice,
    //         i.UnitPrice * i.Quantity
    //     )).ToList()
    // );
}