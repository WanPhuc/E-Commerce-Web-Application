using WebBanHang.Models.DTOs.Seller.Setting;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerSettingService
{
    Task<SellerSettingDto> GetSellerSettingAsync(Guid userId);
    Task UpdateSellerSettingAsync(Guid userId, SellerSettingDto settingDto);
}