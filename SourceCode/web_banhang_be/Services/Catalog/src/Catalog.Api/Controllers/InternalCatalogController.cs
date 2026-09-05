using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using AuraMart.Catalog.Infrastructure.Persistence;
using System.Text.Json;

namespace AuraMart.Catalog.Controllers;

// ============================================================================
// PHASE 6.2 - Internal API cua Catalog cho core app goi (thay viec query
// truc tiep bang catalog). Bao mat: X-Internal-Api-Key.
// ============================================================================
[ApiController]
[AllowAnonymous]
[Route("internal/catalog")]
public class InternalCatalogController : ControllerBase
{
    public static string InternalApiKey { get; set; } = "dev-internal-key";

    private readonly CatalogDbContext _db;
    private readonly IServiceProvider _provider;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web);

    public InternalCatalogController(CatalogDbContext db, IServiceProvider provider, IDistributedCache cache)
    {
        _db = db;
        _provider = provider;
        _cache = cache;
    }

    private bool IsAuthorized() =>
        Request.Headers.TryGetValue("X-Internal-Api-Key", out var key) && key == InternalApiKey;

    private IActionResult Denied() => StatusCode(403, new { error = "forbidden" });

    // GET internal/catalog/products/stats/total
    [HttpGet("products/stats/total")]
    public async Task<IActionResult> TotalProducts()
    {
        if (!IsAuthorized()) return Denied();
        var count = await _db.Products.AsNoTracking().CountAsync();
        return Ok(new { count });
    }

    // GET internal/catalog/products/{id}
    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        if (!IsAuthorized()) return Denied();

        var cacheKey = $"catalog:product:{id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return Content(cached, "application/json");
        }

        var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();

        var result = new { p.Id, p.Name, p.SKU, p.Price, p.DiscountPercent, p.Stock, p.SellerId, Status = p.Status.ToString() };
        var json = System.Text.Json.JsonSerializer.Serialize(result, _jsonOpts);
        await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Ok(result);
    }

    // POST internal/catalog/products/by-ids
    [HttpPost("products/by-ids")]
    public async Task<IActionResult> GetByIds([FromBody] Guid[] ids)
    {
        if (!IsAuthorized()) return Denied();

        var results = new List<object>();
        var missingIds = new List<Guid>();

        foreach (var id in ids)
        {
            var cacheKey = $"catalog:product:{id}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                var deserialized = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(cached, _jsonOpts);
                if (deserialized != null) results.Add(deserialized);
            }
            else
            {
                missingIds.Add(id);
            }
        }

        if (missingIds.Count > 0)
        {
            var dbProducts = await _db.Products.AsNoTracking().Where(p => missingIds.Contains(p.Id)).ToListAsync();
            foreach (var p in dbProducts)
            {
                var item = new { p.Id, p.Name, p.SKU, p.Price, p.DiscountPercent, p.Stock, p.SellerId, Status = p.Status.ToString() };
                results.Add(item);

                var cacheKey = $"catalog:product:{p.Id}";
                var json = System.Text.Json.JsonSerializer.Serialize(item, _jsonOpts);
                await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }

        return Ok(results);
    }

    // GET internal/catalog/products/by-seller/{sellerId}
    [HttpGet("products/by-seller/{sellerId:guid}")]
    public async Task<IActionResult> GetBySeller(Guid sellerId)
    {
        if (!IsAuthorized()) return Denied();
        var list = await _db.Products.AsNoTracking().Where(p => p.SellerId == sellerId).ToListAsync();
        return Ok(list.Select(p => new { p.Id, p.Name, p.SKU, p.Price, p.DiscountPercent, p.Stock, p.SellerId, Status = p.Status.ToString() }));
    }

    // GET internal/catalog/products/count-by-seller/{sellerId}
    [HttpGet("products/count-by-seller/{sellerId:guid}")]
    public async Task<IActionResult> CountBySeller(Guid sellerId)
    {
        if (!IsAuthorized()) return Denied();
        var count = await _db.Products.AsNoTracking().CountAsync(p => p.SellerId == sellerId);
        return Ok(new { count });
    }

    // GET internal/catalog/products/images?ids=a,b,c - anh chinh (fallback anh dau)
    [HttpGet("products/images")]
    public async Task<IActionResult> GetImages([FromQuery] string ids)
    {
        if (!IsAuthorized()) return Denied();
        var list = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList();
        var imgs = await _db.ProductImages.AsNoTracking()
            .Where(i => list.Contains(i.ProductId))
            .OrderBy(i => i.IsMainImage ? 0 : 1)
            .ToListAsync();
        var dict = imgs.GroupBy(i => i.ProductId).ToDictionary(g => g.Key, g => g.First().ImageUrl);
        return Ok(dict);
    }

    // GET internal/catalog/products/low-stock/{sellerId}?take=5
    [HttpGet("products/low-stock/{sellerId:guid}")]
    public async Task<IActionResult> LowStock(Guid sellerId, [FromQuery] int take = 5)
    {
        if (!IsAuthorized()) return Denied();
        var items = await _db.Products.AsNoTracking()
            .Where(p => p.SellerId == sellerId && p.Stock <= p.LowStockThreshold)
            .OrderBy(p => p.Stock)
            .Take(take)
            .Select(p => new { productId = p.Id, productName = p.Name, stockQuantity = p.Stock, sku = p.SKU })
            .ToListAsync();
        return Ok(items);
    }

    // GET internal/catalog/products/recent-ratings/{sellerId}?take=5
    [HttpGet("products/recent-ratings/{sellerId:guid}")]
    public async Task<IActionResult> RecentRatings(Guid sellerId, [FromQuery] int take = 5)
    {
        if (!IsAuthorized()) return Denied();
        var items = await _db.ProductReviews.AsNoTracking()
            .Where(r => r.Product.SellerId == sellerId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(take)
            .Select(r => new
            {
                reviewId = r.Id,
                productId = r.ProductId,
                productName = r.Product.Name,
                customerName = r.DisplayName ?? "Khach hang",
                sku = r.Product.SKU,
                imageUrl = _db.ProductImages.Where(pi => pi.ProductId == r.ProductId)
                    .OrderByDescending(pi => pi.IsMainImage).Select(pi => pi.ImageUrl).FirstOrDefault(),
                rating = r.Rating,
                comment = r.Comment ?? "",
                reviewDate = r.CreatedAt
            })
            .ToListAsync();
        return Ok(items);
    }

    // GET internal/catalog/products/top-selling/{sellerId}?take=6
    // Tra ve thong tin san pham theo SoldCount (duoc cap nhat qua event OrderCompleted)
    [HttpGet("products/top-selling/{sellerId:guid}")]
    public async Task<IActionResult> TopSelling(Guid sellerId, [FromQuery] int take = 6)
    {
        if (!IsAuthorized()) return Denied();
        var items = await _db.Products.AsNoTracking()
            .Where(p => p.SellerId == sellerId && p.SoldCount > 0)
            .OrderByDescending(p => p.SoldCount)
            .Take(take)
            .Select(p => new
            {
                productId = p.Id,
                productName = p.Name,
                sku = p.SKU,
                soldCount = p.SoldCount,
                imageUrl = _db.ProductImages.Where(pi => pi.ProductId == p.Id)
                    .OrderByDescending(pi => pi.IsMainImage).Select(pi => pi.ImageUrl).FirstOrDefault()
            })
            .ToListAsync();
        return Ok(items);
    }

    // GET internal/catalog/products/rating-summary/{sellerId}
    [HttpGet("products/rating-summary/{sellerId:guid}")]
    public async Task<IActionResult> RatingSummary(Guid sellerId)
    {
        if (!IsAuthorized()) return Denied();
        var reviews = _db.ProductReviews.AsNoTracking().Where(r => r.Product.SellerId == sellerId);
        var total = await reviews.CountAsync();
        var avg = await reviews.AnyAsync() ? await reviews.AverageAsync(r => (double)r.Rating) : 0;
        return Ok(new { averageRating = avg, totalReviews = total });
    }

    // POST internal/catalog/demo/seed-catalog?sellerId=...
    // Tao categories + products demo cho seller (goi boi DemoDataSeeder cua core).
    // Tra ve danh sach product de core tao order/cart demo.
    [HttpPost("demo/seed-catalog")]
    public async Task<IActionResult> SeedDemoCatalog([FromQuery] Guid sellerId)
    {
        if (!IsAuthorized()) return Denied();

        if (await _db.Products.AnyAsync(p => p.SKU.StartsWith("DEMO-")))
        {
            var existing = await _db.Products.AsNoTracking().Where(p => p.SKU.StartsWith("DEMO-")).ToListAsync();
            return Ok(existing.Select(p => new { productId = p.Id, sku = p.SKU, name = p.Name, price = p.Price }).ToList());
        }

        var now = DateTime.UtcNow;
        var electronics = await FindOrCreateCategoryAsync("Electronics", now);
        var fashion = await FindOrCreateCategoryAsync("Fashion", now);
        var home = await FindOrCreateCategoryAsync("Home & Living", now);
        await _db.SaveChangesAsync();

        var demoProducts = new[]
        {
            new { Sku = "DEMO-KEYBOARD-001", Name = "Mechanical Keyboard K87", Desc = "Compact mechanical keyboard for work and gaming.", Price = 1190000m, Discount = 8.0, Stock = 42, Cat = electronics },
            new { Sku = "DEMO-MOUSE-001", Name = "Wireless Mouse M2", Desc = "Lightweight wireless mouse with silent clicks.", Price = 399000m, Discount = 12.0, Stock = 80, Cat = electronics },
            new { Sku = "DEMO-HOODIE-001", Name = "Basic Cotton Hoodie", Desc = "Soft cotton hoodie for daily wear.", Price = 459000m, Discount = 15.0, Stock = 35, Cat = fashion },
            new { Sku = "DEMO-LAMP-001", Name = "Minimal Desk Lamp", Desc = "Warm desk lamp for home office setup.", Price = 329000m, Discount = 5.0, Stock = 24, Cat = home }
        };

        var result = new List<object>();
        foreach (var d in demoProducts)
        {
            var product = new Product
            {
                SellerId = sellerId,
                CategoryId = d.Cat.Id,
                SKU = d.Sku,
                Name = d.Name,
                Description = d.Desc,
                Price = d.Price,
                DiscountPercent = d.Discount,
                SoldCount = 0,
                Stock = d.Stock,
                LowStockThreshold = 5,
                Status = ProductStatus.Active,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            _db.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                ImageUrl = d.Sku switch
                {
                    "DEMO-KEYBOARD-001" => "https://images.unsplash.com/photo-1587829741301-dc798b83add3.jpg",
                    "DEMO-MOUSE-001" => "https://images.unsplash.com/photo-1527814050087-3793815479db.jpg",
                    "DEMO-HOODIE-001" => "https://images.unsplash.com/photo-1556821840-3a63f95609a7.jpg",
                    _ => "https://images.unsplash.com/photo-1507473885765-e6ed057f782c.jpg"
                },
                IsMainImage = true,
                CreatedAt = now,
                UpdatedAt = now
            });
            result.Add(new { productId = product.Id, sku = product.SKU, name = product.Name, price = product.Price });
        }
        await _db.SaveChangesAsync();
        return Ok(result);
    }

    private async Task<Category> FindOrCreateCategoryAsync(string name, DateTime now)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Name == name);
        if (category != null) return category;
        category = new Category { Name = name, CreatedAt = now, UpdatedAt = now };
        _db.Categories.Add(category);
        return category;
    }
}
