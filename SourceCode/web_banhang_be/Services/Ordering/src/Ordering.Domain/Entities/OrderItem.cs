using System.ComponentModel.DataAnnotations;

using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Ordering.Domain;

public class OrderItem : Entity
{
    [Required]
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(150)]
    public string ProductName { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = default!;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }
}
