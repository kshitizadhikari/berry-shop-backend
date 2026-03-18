using Domain.Dtos.Order;
using Domain.Entities;

namespace Service.Mappers;

public static class OrderItemMapper
{
    public static OrderItemDto ToDto(this OrderItem item) => new()
    {
        Id = item.Id,
        OrderId = item.OrderId,
        ProductId = item.ProductId,
        ProductName = item.Product?.Name ?? string.Empty,
        ProductImageUrl = item.Product?.ImageUrl,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        Subtotal = item.UnitPrice * item.Quantity
    };

    public static List<OrderItemDto> ToDto(this IEnumerable<OrderItem> items)
        => items.Select(i => i.ToDto()).ToList();
}