namespace Infrastructure.Repositories.IRepositories;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICartRepository Carts { get; }
    ICartItemRepository CartItems { get; }
    IOrderRepository Orders { get; }
    IOrderItemRepository OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}