using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.IRepositories;

namespace Infrastructure.Repositories;

public class OrderRepository(AppDbContext dbContext) : BaseRepository<Order>(dbContext), IOrderRepository 
{
    
}