using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.IRepositories;

namespace Infrastructure.Repositories;

public class CartRepository(AppDbContext dbContext) : BaseRepository<Cart>(dbContext), ICartRepository
{
    
}