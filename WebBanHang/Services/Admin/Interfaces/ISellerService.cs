using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.ViewModels;

namespace WebBanHang.Services.Interfaces;

public interface ISellerService
{
    Task<SellerManagementVM> GetAllSellersAsync();
    Task<SellerDetailDto> GetSellerDetailByIdAsync(Guid sellerId);
}