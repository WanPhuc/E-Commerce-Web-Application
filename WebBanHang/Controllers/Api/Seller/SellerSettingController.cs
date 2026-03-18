using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Setting;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Controllers.Api.Seller;

[ApiController]
[Route("api/v1/rseller/settings")]
[Authorize(Roles = "Seller")]
public class SellerSettingController : ControllerBase
{
    private readonly ISellerSettingService _sellerSettingService;

    public SellerSettingController(ISellerSettingService sellerSettingService)
    {
        _sellerSettingService = sellerSettingService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SellerSettingDto>>> GetSettings()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);

            var settings = await _sellerSettingService.GetSellerSettingAsync(userId);
            return Ok(ApiResponse<SellerSettingDto>.Ok(settings));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SellerSettingDto>.Fail(ex.Message, 404));
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateSettings([FromBody] SellerSettingDto settingDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);

            await _sellerSettingService.UpdateSellerSettingAsync(userId, settingDto);
            return Ok(ApiResponse<string>.Ok("Success", "Settings updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
        }
    }
}