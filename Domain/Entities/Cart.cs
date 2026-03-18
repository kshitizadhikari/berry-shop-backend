using Domain.Entities.Base;

namespace Domain.Entities;

public class Cart : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal Total => Items.Sum(i => i.Product.Price * i.Quantity);
}
