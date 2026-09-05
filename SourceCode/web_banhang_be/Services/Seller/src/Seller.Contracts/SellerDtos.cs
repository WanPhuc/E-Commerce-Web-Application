namespace AuraMart.Seller.Contracts;

public class RegisterSellerRequest
{
    public string ShopName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string? AddressLine { get; set; }
}

public class SellerResponseDto
{
    public Guid Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid AddressId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SellerApplicationResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string? AddressLine { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
