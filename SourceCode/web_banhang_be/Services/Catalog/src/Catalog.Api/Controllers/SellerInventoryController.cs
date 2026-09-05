using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanFucVN.Core.Web.Controllers;


using AuraMart.Catalog.Dtos;
using AuraMart.Catalog.Services;
namespace AuraMart.Catalog.Controllers;
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
