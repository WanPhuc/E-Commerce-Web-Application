using WebBanHang.Models.Enums;

namespace WebBanHang.Models.DTOs;

public class SellerApplicationDto
{
    public Guid Id { get; set; }
    public string ShopName { get; set; }=default!;
    public string Email { get; set; }=default!;
    public SellerApplicationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
