using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Setting;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;

public class SellerSettingService : ISellerSettingService
{
    private readonly ISellerRepository _sellerRepository;
    private readonly ICommonService _commonService;
    public SellerSettingService(ISellerRepository sellerRepository, ICommonService commonService)
    {
        _sellerRepository = sellerRepository;
        _commonService = commonService;
    }
    public async Task<ApiResponse<SellerSettingDto>> GetSellerSettingAsync()
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), false, s => s.User, s => s.Address);
        if (seller == null)
        {
            return ApiResponse<SellerSettingDto>.Fail("Seller not found", 404, ErrorCodes.Seller.NotFound);
        }
        var setting = new SellerSettingDto
        {
            StoreName = seller.StoreName,
            Description = seller.Description,
            RecipientName = seller.User.FullName,
            PhoneNumber = seller.Address.PhoneNumber,
            AddressLine = seller.Address.AddressLine,
            Ward = seller.Address.Ward,
            District = seller.Address.District,
            City = seller.Address.City,
            Email = seller.User.Email
        };
        return ApiResponse<SellerSettingDto>.Success(setting, "Success", 200, SuccessCodes.Seller.SettingsRetrieved);
    }
    public async Task<ApiResponse<string>> UpdateSellerSettingAsync(SellerSettingDto settingDto)
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), true, s => s.User, s => s.Address);
        if (seller == null)
        {
            return ApiResponse<string>.Fail("Seller not found", 404, ErrorCodes.Seller.NotFound);
        }
        seller.StoreName = settingDto.StoreName;
        seller.Description = settingDto.Description;
        seller.User.FullName = settingDto.RecipientName;
        seller.User.Email = settingDto.Email;
        seller.Address.PhoneNumber = settingDto.PhoneNumber;
        seller.Address.AddressLine = settingDto.AddressLine;
        seller.Address.Ward = settingDto.Ward;
        seller.Address.District = settingDto.District;
        seller.Address.City = settingDto.City;

        await _sellerRepository.UpdateAsync(seller);
        return ApiResponse<string>.Success("Success", "Settings updated successfully", 200, SuccessCodes.Seller.SettingsUpdated);
    }
}
