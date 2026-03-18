using Domain.Dtos;
using Domain.Dtos.Cart;
using Domain.Entities;

namespace Service.Mappers;

public static class CartMapper
{
    public static CartItemDto ToDto(this CartItem item) => new()
    {
        Id = item.Id,
        ProductId = item.ProductId,
        ProductName = item.Product.Name,
        ProductImageUrl = item.Product.ImageUrl,
        UnitPrice = item.Product.Price,
        Quantity = item.Quantity,
        Subtotal = item.Product.Price * item.Quantity
    };

    public static CartDto ToDto(this Cart cart) => new()
    {
        Id = cart.Id,
        Items = cart.Items.Select(i => i.ToDto()).ToList(),
        Total = cart.Items.Sum(i => i.Product.Price * i.Quantity)
    };
}