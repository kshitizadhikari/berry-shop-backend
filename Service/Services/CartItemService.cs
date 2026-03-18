using Domain.Dtos.Cart;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Service.Mappers;
using Service.Services.IServices;

namespace Service.Services;

public class CartItemService(IUnitOfWork uow) : ICartItemService
{
    public async Task<CartDto> AddItemAsync(string userId, AddToCartDto request)
    {
        var product = await uow.Products.GetByIdAsync(request.ProductId)
                      ?? throw new NotFoundException("Product", request.ProductId);

        if (!product.IsActive)
            throw new BadRequestException("Product is not available.");

        if (product.StockQuantity < request.Quantity)
            throw new BadRequestException($"Only {product.StockQuantity} units in stock.");

        var cart = await GetOrCreateCartAsync(userId);

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (existing is not null)
        {
            var newQty = existing.Quantity + request.Quantity;
            if (product.StockQuantity < newQty)
                throw new BadRequestException(
                    $"Cannot add {request.Quantity} more. Only {product.StockQuantity} in stock.");

            existing.Quantity = newQty;
            existing.UpdatedAt = DateTime.UtcNow;
            uow.CartItems.Update(existing);
        }
        else
        {
            await uow.CartItems.AddAsync(new CartItem
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        uow.Carts.Update(cart);
        await uow.SaveChangesAsync();

        return (await LoadCartAsync(cart.Id)).ToDto();
    }

    public async Task<CartDto> UpdateItemAsync(string userId, Guid cartItemId, UpdateCartItemDto request)
    {
        var cart = await GetOrCreateCartAsync(userId);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
                   ?? throw new NotFoundException("Cart item", cartItemId);

        var product = await uow.Products.GetByIdAsync(item.ProductId)
                      ?? throw new NotFoundException("Product", item.ProductId);

        if (product.StockQuantity < request.Quantity)
            throw new BadRequestException($"Only {product.StockQuantity} units in stock.");

        item.Quantity = request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;

        uow.CartItems.Update(item);
        uow.Carts.Update(cart);
        await uow.SaveChangesAsync();

        return (await LoadCartAsync(cart.Id)).ToDto();
    }

    public async Task<CartDto> RemoveItemAsync(string userId, Guid cartItemId)
    {
        var cart = await GetOrCreateCartAsync(userId);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
                   ?? throw new NotFoundException("Cart item", cartItemId);

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;

        uow.CartItems.Update(item);
        uow.Carts.Update(cart);
        await uow.SaveChangesAsync();

        return (await LoadCartAsync(cart.Id)).ToDto();
    }

    public async Task<CartDto> ClearCartAsync(string userId)
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

        return (await LoadCartAsync(cart.Id)).ToDto();
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