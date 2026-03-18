using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.IRepositories;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext dbContext) : BaseRepository<Product>(dbContext), IProductRepository
{
    
}