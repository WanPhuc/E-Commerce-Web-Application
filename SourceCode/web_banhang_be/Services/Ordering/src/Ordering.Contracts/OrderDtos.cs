namespace AuraMart.Ordering.Contracts;

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class OrderShippingAddressDto
{
    public string RecipientName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class CreateOrderRequest
{
    public Guid SellerId { get; set; }
    public Guid AddressId { get; set; }
    public OrderShippingAddressDto ShippingAddress { get; set; } = default!;
    public List<CreateOrderItemDto> Items { get; set; } = [];
}

public class OrderItemResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Guid AddressId { get; set; }
    public OrderShippingAddressDto ShippingAddress { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = [];
}
