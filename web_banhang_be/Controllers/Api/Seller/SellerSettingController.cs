using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Filters;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Setting;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Controllers.Api.Seller;

[ApiController]
[Route("api/v1/rseller/settings")]
[AuthorizeRole("Seller")]
public class SellerSettingController : BaseController
{
    private readonly ISellerSettingService _sellerSettingService;

    public SellerSettingController(ISellerSettingService sellerSettingService)
    {
        _sellerSettingService = sellerSettingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _sellerSettingService.GetSellerSettingAsync();
        return BaseResult(settings);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> UpdateSettings([FromBody] SellerSettingDto settingDto)
    {
        var result = await _sellerSettingService.UpdateSellerSettingAsync(settingDto);
        return BaseResult(result);
    }
}
