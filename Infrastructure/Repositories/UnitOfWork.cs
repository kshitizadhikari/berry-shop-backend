using Infrastructure.Data;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    private IProductRepository? _products;
    private ICartRepository? _carts;
    private ICartItemRepository? _cartItems;
    private IOrderRepository? _orders;
    private IOrderItemRepository? _orderItems;

    public IProductRepository Products
        => _products ??= new ProductRepository(context);

    public ICartRepository Carts
        => _carts ??= new CartRepository(context);

    public ICartItemRepository CartItems
        => _cartItems ??= new CartItemRepository(context);

    public IOrderRepository Orders
        => _orders ??= new OrderRepository(context);

    public IOrderItemRepository OrderItems
        => _orderItems ??= new OrderItemRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync()
        => _transaction = await context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        try
        {
            await context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await _transaction!.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }
}