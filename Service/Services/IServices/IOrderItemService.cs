using Domain.Dtos.Order;

namespace Service.Services.IServices;

public interface IOrderItemService
{
    Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(string userId, Guid orderId);
    Task<OrderItemDto> GetByIdAsync(string userId, Guid orderItemId);
    Task<IEnumerable<OrderItemDto>> GetAllAsync();
}