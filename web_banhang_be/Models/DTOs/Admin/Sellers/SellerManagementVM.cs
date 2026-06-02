using WebBanHang.Models.DTOs;
using WebBanHang.Models.Common;

namespace WebBanHang.Models.DTOs.Admin.Sellers;

public class SellerManagementVM
{
    public int PendingSellerApplications { get; set; }
    public PagedResult<SellerDto> ApprovedSellers { get; set; } = new();
    public PagedResult<SellerApplicationDto> SellerApplications { get; set; } = new();
}
