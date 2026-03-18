using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.IRepositories;

namespace Infrastructure.Repositories;

public class CartItemRepository(AppDbContext dbContext) : BaseRepository<CartItem>(dbContext), ICartItemRepository
{
    
}