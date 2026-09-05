using System.ComponentModel.DataAnnotations;

namespace AuraMart.Ordering.Domain;

public class OrderShippingAddress
{
    [MaxLength(100)]
    public string RecipientName { get; set; } = default!;

    [MaxLength(15)]
    public string PhoneNumber { get; set; } = default!;

    [MaxLength(200)]
    public string? AddressLine { get; set; }

    public string Ward { get; set; } = default!;
    public string District { get; set; } = default!;
    public string City { get; set; } = default!;
}
