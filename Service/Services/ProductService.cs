using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.QueryParameters;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Service.Mappers;
using Service.Services.IServices;

namespace Service.Services;

public class ProductService(IUnitOfWork uow) : IProductService
{
    public async Task<PagedResultDto<ProductDto>> GetAllAsync(ProductQp query)
    {
        var q = uow.Products.Query();

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(p => p.Name.Contains(query.Search) || p.Description.Contains(query.Search));

        if (!string.IsNullOrWhiteSpace(query.Category))
            q = q.Where(p => p.Category == query.Category);

        if (query.MinPrice.HasValue) q = q.Where(p => p.Price >= query.MinPrice.Value);
        if (query.MaxPrice.HasValue) q = q.Where(p => p.Price <= query.MaxPrice.Value);

        q = (query.SortBy.ToLower(), query.SortDesc) switch
        {
            ("price", false) => q.OrderBy(p => p.Price),
            ("price", true) => q.OrderByDescending(p => p.Price),
            ("createdat", false) => q.OrderBy(p => p.CreatedAt),
            ("createdat", true) => q.OrderByDescending(p => p.CreatedAt),
            (_, false) => q.OrderBy(p => p.Name),
            (_, true) => q.OrderByDescending(p => p.Name),
        };

        var totalCount = await q.CountAsync();
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => p.ToDto())
            .ToListAsync();

        return PagedResultDto<ProductDto>.Create(items, totalCount, query.Page, query.PageSize);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await uow.Products.GetByIdAsync(id)
                      ?? throw new NotFoundException("Product", id);

        return product.ToDto();
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
        => await uow.Products.Query()
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    public async Task<ProductDto> CreateAsync(CreateProductDto request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Category = request.Category,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Products.AddAsync(product);
        await uow.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto request)
    {
        var product = await uow.Products.GetByIdAsync(id)
                      ?? throw new NotFoundException("Product", id);

        if (request.Name is not null) product.Name = request.Name;
        if (request.Description is not null) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity.Value;
        if (request.Category is not null) product.Category = request.Category;
        if (request.ImageUrl is not null) product.ImageUrl = request.ImageUrl;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;
        product.UpdatedAt = DateTime.UtcNow;

        uow.Products.Update(product);
        await uow.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await uow.Products.GetByIdAsync(id)
                      ?? throw new NotFoundException("Product", id);

        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        uow.Products.Update(product);
        await uow.SaveChangesAsync();
    }
}