using WebBanHang.Models.Enums;

namespace WebBanHang.Models.DTOs;
public class SellerDto
{
    public Guid Id { get; set; }
    public string StoreName { get; set; }=default!;
    public string? Description { get; set; }
    public string Email { get; set; }=default!;
    public DateTime CreatedAt { get; set; }
    public SellerApplicationStatus Status { get; set; }
    public SellerAdressDto Address { get; set; }=default!;
    
}

public class SellerAdressDto
{
    public string PhoneNumber { get; set; }=default!;
    public string District { get; set; }=default!;
    public string City { get; set; }=default!;
}