using Domain.Dtos;
using Domain.Dtos.Cart;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Service.Mappers;
using Service.Services.IServices;

namespace Service.Services;

public class CartService(IUnitOfWork uow) : ICartService
{
    public async Task<CartDto> GetCartAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return cart.ToDto();
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);

        foreach (var item in cart.Items)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            uow.CartItems.Update(item);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        uow.Carts.Update(cart);
        await uow.SaveChangesAsync();
    }

    private async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await uow.Carts.Query()
            .Include(c => c.Items.Where(i => !i.IsDeleted))
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is not null) return cart;

        cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
        await uow.Carts.AddAsync(cart);
        await uow.SaveChangesAsync();

        return cart;
    }

    private async Task<Cart> LoadCartAsync(Guid cartId)
        => await uow.Carts.Query()
            .Include(c => c.Items.Where(i => !i.IsDeleted))
            .ThenInclude(i => i.Product)
            .FirstAsync(c => c.Id == cartId);
}