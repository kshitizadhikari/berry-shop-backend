using Domain.Dtos;
using Domain.QueryParameters;

namespace Service.Services.IServices;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetAllAsync(ProductQp query);
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<ProductDto> CreateAsync(CreateProductDto request);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto request);
    Task DeleteAsync(Guid id);
}