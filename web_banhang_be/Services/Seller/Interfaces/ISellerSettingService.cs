using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Setting;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerSettingService
{
    Task<ApiResponse<SellerSettingDto>> GetSellerSettingAsync( );
    Task<ApiResponse<string>> UpdateSellerSettingAsync(  SellerSettingDto settingDto);
}
