using Domain.Dtos;
using Domain.Dtos.Cart;

namespace Service.Services.IServices;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId);
    Task ClearCartAsync(string userId);
}