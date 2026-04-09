using WebBanHang.Models.DTOs;

namespace WebBanHang.Models.DTOs.Admin.Sellers;

public class SellerManagementVM
{
    public int PendingSellerApplications { get; set; }
    public List<SellerDto> ApprovedSellers { get; set; } = new();
    public List<SellerApplicationDto> SellerApplications { get; set; } = new();
}
