using Domain.Dtos.Order;
using Domain.Entities;

namespace Service.Mappers;

public static class OrderMapper
{
    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        Status = order.Status,
        ShippingAddress = order.ShippingAddress,
        TotalAmount = order.TotalAmount,
        PlacedAt = order.CreatedAt,
        Items = order.Items.Select(i => i.ToDto()).ToList()
    };
}