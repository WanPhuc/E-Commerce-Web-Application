using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Order;
using WebBanHang.Services.Seller.Interfaces;
using System.Security.Claims;
using WebBanHang.Models.Enums;
using WebBanHang.Filters;
using WebBanHang.Controllers.Api.Base;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/orders")]
[AuthorizeRole("Seller")]
public class SellerOrderController : BaseController
{
    private readonly ISellerOrderService _sellerOrderService;
    public SellerOrderController(ISellerOrderService sellerOrderService)
    {
        _sellerOrderService = sellerOrderService;
    }
    [HttpGet]
    public async Task<IActionResult> GetSellerOrders([FromQuery] PagedRequest request)
    {
        
        
        var orders = await _sellerOrderService.GetSellerOrdersAsync(request);
        return BaseResult(orders);
        
    }
    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetDetailSellerOrderById(Guid orderId)
    {
        var order = await _sellerOrderService.GetDetailSellerOrderByIdAsync(orderId);
        return BaseResult(order);
    }
    [HttpPatch("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateSellerOrderStatus(Guid orderId, OrderStatus newStatus)
    {
        var result = await _sellerOrderService.UpdateSellerOrderStatusAsync(orderId, newStatus);
        return BaseResult(result);
    }
}
