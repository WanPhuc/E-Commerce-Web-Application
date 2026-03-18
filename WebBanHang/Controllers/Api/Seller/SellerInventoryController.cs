using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Inventory;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/inventory")]
[Authorize(Roles = "Seller")]
public class SellerInventoryController : ControllerBase
{
    private readonly ISellerInventoryService _sellerInventoryService;
    public SellerInventoryController(ISellerInventoryService sellerInventoryService)
    {
        _sellerInventoryService = sellerInventoryService;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SellerInventoryDto>>>> GetSellerInventory()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        var inventory = await _sellerInventoryService.GetSellerInventoryAsync(userId);
        return Ok(ApiResponse<List<SellerInventoryDto>>.Ok(inventory));
    }
    [HttpPatch("{productId:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateSellerInventory(Guid productId, [FromBody] UpdateSellerInventoryDto dto)
    {
        try
        {
            await _sellerInventoryService.UpdateSellerInventoryAsync(productId, dto);
            return Ok(ApiResponse<string>.Ok("Inventory updated successfully."));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<string>.Fail("Product not found."));
        }
    }
}