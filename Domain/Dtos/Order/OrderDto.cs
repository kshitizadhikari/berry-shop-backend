using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime PlacedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class PlaceOrderDto
{
    [Required] [MaxLength(300)] public string ShippingAddress { get; set; } = string.Empty;
}