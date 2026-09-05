using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuraMart.Ordering.Infrastructure.Persistence;
using AuraMart.Ordering.Domain;

namespace AuraMart.Ordering.Api.Controllers;

[ApiController]
[Route("internal/ordering")]
public class InternalOrderingController : ControllerBase
{
    public static string InternalApiKey { get; set; } = "dev-internal-key";
    private readonly OrderingDbContext _db;

    public InternalOrderingController(OrderingDbContext db) => _db = db;

    private bool IsAuthorized() =>
        Request.Headers.TryGetValue("X-Internal-Api-Key", out var key) && key == InternalApiKey;

    private IActionResult Denied() => StatusCode(403, new { error = "forbidden" });

    // GET internal/ordering/orders/{id}
    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var order = await _db.Orders.AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return Ok(new
        {
            order.Id, order.UserId, order.CustomerName, order.TotalAmount,
            Status = order.Status, order.SellerId,
            Items = order.Items.Select(i => new { i.ProductId, i.Quantity, i.Price })
        });
    }

    // GET internal/ordering/orders/seller/{sellerId}
    [HttpGet("orders/seller/{sellerId:guid}")]
    public async Task<IActionResult> GetSellerOrders(Guid sellerId, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        if (!IsAuthorized()) return Denied();
        var orders = await _db.Orders.AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.SellerId == sellerId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(o => new
            {
                o.Id, o.UserId, o.CustomerName, o.TotalAmount,
                Status = o.Status, o.SellerId, o.CreatedAt, o.UpdatedAt, o.PaidAt, o.CompletedAt,
                ShippingAddress = new { o.ShippingAddress.PhoneNumber, o.ShippingAddress.City },
                Items = o.Items.Select(i => new { i.ProductId, i.ProductName, i.Sku, i.Quantity, i.Price })
            })
            .ToListAsync();
        return Ok(orders);
    }

    // POST internal/ordering/orders
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest req)
    {
        if (!IsAuthorized()) return Denied();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = req.BuyerId,
            CustomerName = req.CustomerName,
            CustomerEmail = req.CustomerEmail,
            AddressId = req.AddressId,
            ShippingAddress = new OrderShippingAddress
            {
                RecipientName = req.RecipientName,
                PhoneNumber = req.PhoneNumber,
                AddressLine = req.AddressLine,
                Ward = req.Ward,
                District = req.District,
                City = req.City
            },
            TotalAmount = req.TotalAmount,
            Status = "Pending",
            SellerId = req.SellerId,
            Items = req.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Sku = i.Sku,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return Ok(new { order.Id, order.Status });
    }

    // PATCH internal/ordering/orders/{id}/status
    [HttpPatch("orders/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest req)
    {
        if (!IsAuthorized()) return Denied();
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();

        order.Status = req.Status;
        order.UpdatedAt = DateTime.UtcNow;
        if (req.Status == "Paid") order.PaidAt = DateTime.UtcNow;
        if (req.Status == "Completed") order.CompletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { order.Id, order.Status });
    }

    public record CreateOrderRequest(
        Guid BuyerId, string CustomerName, string CustomerEmail, Guid AddressId,
        string RecipientName, string PhoneNumber, string AddressLine, string Ward, string District, string City,
        decimal TotalAmount, Guid SellerId, List<OrderItemRequest> Items);
    public record OrderItemRequest(Guid ProductId, string ProductName, string Sku, int Quantity, decimal Price);
    public record UpdateStatusRequest(string Status);
}
