using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Order;
using WebBanHang.Services.Seller.Interfaces;
using System.Security.Claims;
using WebBanHang.Models.Enums;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/orders")]
[Authorize(Roles = "Seller")]
public class SellerOrderController : ControllerBase
{
    private readonly ISellerOrderService _sellerOrderService;
    public SellerOrderController(ISellerOrderService sellerOrderService)
    {
        _sellerOrderService = sellerOrderService;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SellerOrderDto>>>> GetSellerOrders()
    {
        try{
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId=Guid.Parse(userIdClaim.Value);
        var orders = await _sellerOrderService.GetSellerOrdersAsync(userId);
        return Ok(ApiResponse<List<SellerOrderDto>>.Ok(orders));
        }catch(KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<List<SellerOrderDto>>.Fail(ex.Message,404));
        }
    }
    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<ApiResponse<SellerOrderDto>>> GetDetailSellerOrderById(Guid orderId)
    {
        try{
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId=Guid.Parse(userIdClaim.Value);
            var order = await _sellerOrderService.GetDetailSellerOrderByIdAsync(orderId,userId);
            return Ok(ApiResponse<SellerOrderDto>.Ok(order));
        }catch(KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SellerOrderDto>.Fail(ex.Message,404));
        }
    }
    [HttpPatch("{orderId:guid}/status")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateSellerOrderStatus(Guid orderId, OrderStatus newStatus)
    {
        try{
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        await _sellerOrderService.UpdateSellerOrderStatusAsync(userId, orderId, newStatus);
        return Ok(ApiResponse<string>.Ok("Order status updated successfully."));
        }catch(KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message,404));
        }catch(InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message,400));
        }
    }
}