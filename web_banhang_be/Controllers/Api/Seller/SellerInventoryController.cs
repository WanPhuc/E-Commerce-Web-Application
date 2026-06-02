using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Filters;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Inventory;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/inventory")]
[AuthorizeRole("Seller")]
public class SellerInventoryController : BaseController
{
    private readonly ISellerInventoryService _sellerInventoryService;
    public SellerInventoryController(ISellerInventoryService sellerInventoryService)
    {
        _sellerInventoryService = sellerInventoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetSellerInventory([FromQuery] PagedRequest request, [FromQuery] string? filter = null)
    {
        
        var inventory = await _sellerInventoryService.GetSellerInventoryAsync(request, filter);
        return BaseResult(inventory);
    }
    [HttpPatch("{productId:guid}")]
    public async Task<IActionResult> UpdateSellerInventory(Guid productId, [FromBody] UpdateSellerInventoryDto dto)
    {
        var result = await _sellerInventoryService.UpdateSellerInventoryAsync(productId, dto);
        return BaseResult(result);
    }
}
