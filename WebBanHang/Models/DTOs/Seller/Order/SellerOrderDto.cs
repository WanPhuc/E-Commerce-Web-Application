using WebBanHang.Models.Enums;

namespace WebBanHang.Models.DTOs.Seller.Order;
public class SellerOrderDto
{
    public Guid Id{ get; set; }
    public string UserName { get; set; }= default!;
    public string PhoneNumber { get; set; }= default!;
    public string City { get; set; }= default!;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string PaymentMethod { get; set; }= default!;
    public PaymentStatus PaymentStatus { get; set; }
    public List<SellerOrderItemDto> Items { get; set; }= new List<SellerOrderItemDto>();

}

public class SellerOrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }= default!;
    public string ImageUrl { get; set; }= default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice => Quantity * Price;
}