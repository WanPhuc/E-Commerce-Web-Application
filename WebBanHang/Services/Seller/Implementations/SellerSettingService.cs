using WebBanHang.Models.DTOs.Seller.Setting;
using WebBanHang.Repositories;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;
public class SellerSettingService : ISellerSettingService
{
    private readonly ISellerRepository _sellerRepository;
    public SellerSettingService(ISellerRepository sellerRepository)
    {
        _sellerRepository = sellerRepository;
    }
    public async Task<SellerSettingDto> GetSellerSettingAsync(Guid userId)
    {
        var seller = await _sellerRepository.GetSellerByUserIdAsync(userId);
        if (seller == null)
        {
            throw new KeyNotFoundException("Seller not found");
        }
        return new SellerSettingDto
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
    }
    public async Task UpdateSellerSettingAsync(Guid userId, SellerSettingDto settingDto)
    {
        var seller = await _sellerRepository.GetSellerByUserIdAsync(userId);
        if (seller == null)
        {
            throw new KeyNotFoundException("Seller not found");
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
        await _sellerRepository.SaveChangesAsync();
    }
}