using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuraMart.Cart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : BaseController
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var result = await _cartService.GetCartAsync(userId);
        return BaseResult(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartRequest req)
    {
        var userId = GetUserId();
        var result = await _cartService.AddItemAsync(userId, req);
        return BaseResult(result);
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid itemId, [FromBody] UpdateCartItemRequest req)
    {
        var userId = GetUserId();
        var result = await _cartService.UpdateItemAsync(userId, itemId, req);
        return BaseResult(result);
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId)
    {
        var userId = GetUserId();
        var result = await _cartService.RemoveItemAsync(userId, itemId);
        return BaseResult(result);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        var userId = GetUserId();
        var result = await _cartService.ClearCartAsync(userId);
        return BaseResult(result);
    }
}
