using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuraMart.Ordering.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Fail("Chưa đăng nhập", 401));
        }

        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Customer";
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";

        var result = await _orderService.CreateOrderAsync(userId, userName, userEmail, request);
        return BaseResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Fail("Chưa đăng nhập", 401));
        }

        var result = await _orderService.GetOrderByIdAsync(id, userId);
        return BaseResult(result);
    }

    [HttpGet("my-orders")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Fail("Chưa đăng nhập", 401));
        }

        var result = await _orderService.GetMyOrdersAsync(userId);
        return BaseResult(result);
    }

    [HttpGet("seller/{sellerId:guid}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetSellerOrders(Guid sellerId)
    {
        var result = await _orderService.GetSellerOrdersAsync(sellerId);
        return BaseResult(result);
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Fail("Chưa đăng nhập", 401));
        }

        var result = await _orderService.CancelOrderAsync(id, userId, request.Reason ?? "Người mua hủy đơn");
        return BaseResult(result);
    }

    public record CancelOrderRequest(string? Reason);
}
