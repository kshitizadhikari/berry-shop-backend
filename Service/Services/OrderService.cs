using Domain.Dtos.Order;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Service.Mappers;
using Service.Services.IServices;

namespace Service.Services;

public class OrderService(IUnitOfWork uow) : IOrderService
{
    public async Task<OrderDto> PlaceOrderAsync(string userId, PlaceOrderDto request)
    {
        var cart = await uow.Carts.Query()
            .Include(c => c.Items.Where(i => !i.IsDeleted))
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null || !cart.Items.Any())
            throw new BadRequestException("Your cart is empty.");

        await uow.BeginTransactionAsync();

        foreach (var item in cart.Items)
        {
            if (!item.Product.IsActive)
                throw new BadRequestException($"'{item.Product.Name}' is no longer available.");

            if (item.Product.StockQuantity < item.Quantity)
                throw new BadRequestException(
                    $"Insufficient stock for '{item.Product.Name}'. Available: {item.Product.StockQuantity}.");

            item.Product.StockQuantity -= item.Quantity;
            item.Product.UpdatedAt = DateTime.UtcNow;
            uow.Products.Update(item.Product);
        }

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress,
            TotalAmount = cart.Items.Sum(i => i.Product.Price * i.Quantity),
            CreatedAt = DateTime.UtcNow,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        await uow.Orders.AddAsync(order);

        foreach (var item in cart.Items)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            uow.CartItems.Update(item);
        }

        await uow.CommitTransactionAsync();

        return await LoadOrderAsync(order.Id);
    }

    public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(string userId)
    {
        var orders = await uow.Orders.Query()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(o => o.ToDto());
    }

    public async Task<OrderDto> GetByIdAsync(string userId, Guid orderId)
    {
        var order = await uow.Orders.Query()
                        .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(o => o.Id == orderId)
                    ?? throw new NotFoundException("Order", orderId);

        if (order.UserId != userId)
            throw new UnauthorizedException("You do not have access to this order.");

        return order.ToDto();
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await uow.Orders.Query()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(o => o.ToDto());
    }

    private async Task<OrderDto> LoadOrderAsync(Guid orderId)
        => (await uow.Orders.Query()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstAsync(o => o.Id == orderId))
            .ToDto();
}