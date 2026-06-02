using WebBanHang.Models.Enums;

namespace WebBanHang.Models.DTOs.Sellers;

public class SellerSummaryDto
{
    public Guid Id { get; set; }
    public string ShopName { get; set; } = default!;
    public SellerApplicationStatus Status { get; set; }
}
