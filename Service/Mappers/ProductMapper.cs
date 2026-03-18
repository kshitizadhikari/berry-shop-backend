using Domain.Dtos;
using Domain.Entities;

namespace Service.Mappers;

public static class ProductMapper
{
    // Product -> ProductDto
    public static ProductDto ToDto(this Product product) => new()
    {
        Id = product.Id.ToString(),
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        StockQuantity = product.StockQuantity,
        Category = product.Category,
        ImageUrl = product.ImageUrl,
        IsActive = product.IsActive
    };

    public static List<ProductDto> ToDto(this IEnumerable<Product> products)
        => products.Select(p => p.ToDto()).ToList();

    // CreateProductDto -> Product
    public static Product ToEntity(this CreateProductDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        StockQuantity = dto.StockQuantity,
        Category = dto.Category,
        ImageUrl = dto.ImageUrl,
        CreatedAt = DateTime.UtcNow
    };

    // UpdateProductDto -> existing Product
    public static void UpdateEntity(this UpdateProductDto dto, Product product)
    {
        if (dto.Name is not null) product.Name = dto.Name;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.StockQuantity.HasValue) product.StockQuantity = dto.StockQuantity.Value;
        if (dto.Category is not null) product.Category = dto.Category;
        if (dto.ImageUrl is not null) product.ImageUrl = dto.ImageUrl;
        if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;
        product.UpdatedAt = DateTime.UtcNow;
    }
}