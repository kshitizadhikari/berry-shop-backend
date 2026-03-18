using Domain.Dtos.Order;
using Domain.Exceptions;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Service.Mappers;
using Service.Services.IServices;

namespace Service.Services;

public class OrderItemService(IUnitOfWork uow) : IOrderItemService
{
    public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(string userId, Guid orderId)
    {
        var order = await uow.Orders.Query()
                        .FirstOrDefaultAsync(o => o.Id == orderId)
                    ?? throw new NotFoundException("Order", orderId);

        if (order.UserId != userId)
            throw new UnauthorizedException("You do not have access to this order.");

        var items = await uow.OrderItems.Query()
            .Include(i => i.Product)
            .Where(i => i.OrderId == orderId)
            .ToListAsync();

        return items.ToDto();
    }

    public async Task<OrderItemDto> GetByIdAsync(string userId, Guid orderItemId)
    {
        var item = await uow.OrderItems.Query()
                       .Include(i => i.Product)
                       .Include(i => i.Order)
                       .FirstOrDefaultAsync(i => i.Id == orderItemId)
                   ?? throw new NotFoundException("Order item", orderItemId);

        if (item.Order.UserId != userId)
            throw new UnauthorizedException("You do not have access to this order item.");

        return item.ToDto();
    }

    public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
    {
        var items = await uow.OrderItems.Query()
            .Include(i => i.Product)
            .Include(i => i.Order)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return items.ToDto();
    }
}