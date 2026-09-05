using System.ComponentModel.DataAnnotations;

using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Seller.Domain;

public class SellerApplication : Entity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(150)]
    public string ShopName { get; set; } = default!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public string PhoneNumber { get; set; } = default!;

    [Required]
    public string City { get; set; } = default!;

    public string District { get; set; } = default!;
    public string Ward { get; set; } = default!;
    public string? AddressLine { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    public DateTime? ReviewedAt { get; set; }
}
