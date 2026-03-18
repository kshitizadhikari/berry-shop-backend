using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos;

public class ProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    [Required] [MaxLength(200)] public string Name { get; set; } = string.Empty;

    [Required] public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }

    [Required] [MaxLength(100)] public string Category { get; set; } = string.Empty;

    [MaxLength(500)] public string? ImageUrl { get; set; }
}

public class UpdateProductDto
{
    [MaxLength(200)] public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int? StockQuantity { get; set; }

    [MaxLength(100)] public string? Category { get; set; }

    [MaxLength(500)] public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }
}