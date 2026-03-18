using Domain.Dtos.Cart;

namespace Service.Services.IServices;

public interface ICartItemService
{
    Task<CartDto> AddItemAsync(string userId, AddToCartDto request);
    Task<CartDto> UpdateItemAsync(string userId, Guid cartItemId, UpdateCartItemDto request);
    Task<CartDto> RemoveItemAsync(string userId, Guid cartItemId);
    Task<CartDto> ClearCartAsync(string userId);
}