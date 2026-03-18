using WebBanHang.Models.DTOs;

namespace WebBanHang.Models.ViewModels;

public class SellerManagementVM
{
    public int PendingSellerApplications { get; set; }
    public List<SellerDto> ApprovedSellers { get; set; } = new();
    public List<SellerApplicationDto> SellerApplications { get; set; } = new();
}
