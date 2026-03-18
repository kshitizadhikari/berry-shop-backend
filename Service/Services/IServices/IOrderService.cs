using Domain.Dtos.Order;

namespace Service.Services.IServices;

public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(string userId, PlaceOrderDto request);
    Task<IEnumerable<OrderDto>> GetMyOrdersAsync(string userId);
    Task<OrderDto> GetByIdAsync(string userId, Guid orderId);
    Task<IEnumerable<OrderDto>> GetAllAsync();
}