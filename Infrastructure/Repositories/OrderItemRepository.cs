using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.IRepositories;

namespace Infrastructure.Repositories;

public class OrderItemRepository(AppDbContext dbContext) : BaseRepository<OrderItem>(dbContext), IOrderItemRepository 
{
    
}