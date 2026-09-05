using System.ComponentModel.DataAnnotations;

using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Seller.Domain;

public class Seller : Entity
{
    [Required]
    [MaxLength(100)]
    public string StoreName { get; set; } = default!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public string Status { get; set; } = "Approved";

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid AddressId { get; set; }
}
