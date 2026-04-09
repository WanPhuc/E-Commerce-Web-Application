using WebBanHang.Models.DTOs.Admin.Sellers;
using WebBanHang.Models.DTOs.Sellers;

namespace WebBanHang.Services.Interfaces;

public interface ISellerService
{
    Task<SellerManagementVM> GetAllSellersAsync();
    Task<SellerDetailDto> GetSellerDetailByIdAsync(Guid sellerId);
}