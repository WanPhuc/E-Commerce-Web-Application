using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuraMart.Seller.Infrastructure.Persistence;

namespace AuraMart.Seller.Api.Controllers;

[ApiController]
[Route("internal/sellers")]
public class InternalSellerController : ControllerBase
{
    public static string InternalApiKey { get; set; } = "dev-internal-key";
    private readonly SellerDbContext _db;

    public InternalSellerController(SellerDbContext db) => _db = db;

    private bool IsAuthorized() =>
        Request.Headers.TryGetValue("X-Internal-Api-Key", out var key) && key == InternalApiKey;

    private IActionResult Denied() => StatusCode(403, new { error = "forbidden" });

    // GET internal/sellers/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var seller = await _db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (seller == null) return NotFound();
        return Ok(new { seller.Id, seller.StoreName, seller.Description, seller.UserId, seller.AddressId, Status = seller.Status, seller.CreatedAt });
    }

    // GET internal/sellers/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        if (!IsAuthorized()) return Denied();
        var seller = await _db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId);
        if (seller == null) return NotFound();
        return Ok(new { seller.Id, seller.StoreName, seller.Description, seller.UserId, seller.AddressId, Status = seller.Status, seller.CreatedAt });
    }

    // GET internal/sellers/by-user-ids?ids=a,b,c
    [HttpGet("by-user-ids")]
    public async Task<IActionResult> GetByUserIds([FromQuery] string ids)
    {
        if (!IsAuthorized()) return Denied();
        var userIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList();
        var sellers = await _db.Sellers.AsNoTracking().Where(s => userIds.Contains(s.UserId)).ToListAsync();
        return Ok(sellers.Select(s => new { s.Id, s.UserId, s.StoreName, s.Status }));
    }

    // GET internal/sellers/count
    [HttpGet("count")]
    public async Task<IActionResult> Count()
    {
        if (!IsAuthorized()) return Denied();
        var count = await _db.Sellers.AsNoTracking().CountAsync();
        return Ok(new { count });
    }

    // GET internal/sellers/stats/created-before?before=2024-01-01
    [HttpGet("stats/created-before")]
    public async Task<IActionResult> CountCreatedBefore([FromQuery] DateTime before)
    {
        if (!IsAuthorized()) return Denied();
        var count = await _db.Sellers.AsNoTracking().CountAsync(s => s.CreatedAt < before);
        return Ok(new { count });
    }

    // GET internal/sellers/stats/creation-dates?from=&to=
    [HttpGet("stats/creation-dates")]
    public async Task<IActionResult> GetCreationDates([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (!IsAuthorized()) return Denied();
        var dates = await _db.Sellers.AsNoTracking()
            .Where(s => s.CreatedAt >= from && s.CreatedAt < to)
            .Select(s => s.CreatedAt.Date)
            .Distinct()
            .ToListAsync();
        return Ok(dates);
    }

    // GET internal/sellers/applications/{id}
    [HttpGet("applications/{id:guid}")]
    public async Task<IActionResult> GetApplication(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var app = await _db.SellerApplications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (app == null) return NotFound();
        return Ok(new { app.Id, app.UserId, app.ShopName, app.Description, app.PhoneNumber, app.City, app.District, app.Ward, app.AddressLine, Status = app.Status, app.CreatedAt, app.ReviewedAt });
    }

    // GET internal/sellers/applications/pending
    [HttpGet("applications/pending")]
    public async Task<IActionResult> GetPendingApplications()
    {
        if (!IsAuthorized()) return Denied();
        var apps = await _db.SellerApplications.AsNoTracking()
            .Where(a => a.Status == "Pending")
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new { a.Id, a.UserId, a.ShopName, Status = a.Status, a.CreatedAt })
            .ToListAsync();
        return Ok(apps);
    }

    // POST internal/sellers/applications/{id}/approve
    [HttpPost("applications/{id:guid}/approve")]
    public async Task<IActionResult> ApproveApplication(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var app = await _db.SellerApplications.FindAsync(id);
        if (app == null) return NotFound();
        if (app.Status != "Pending") return BadRequest(new { error = "Only pending applications can be approved." });

        app.Status = "Approved";
        app.ReviewedAt = DateTime.UtcNow;

        var seller = new Domain.Seller
        {
            UserId = app.UserId,
            StoreName = app.ShopName,
            Description = app.Description,
            Status = "Approved",
            AddressId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        _db.Sellers.Add(seller);
        await _db.SaveChangesAsync();
        return Ok(new { seller.Id, seller.StoreName });
    }

    // POST internal/sellers/applications/{id}/reject
    [HttpPost("applications/{id:guid}/reject")]
    public async Task<IActionResult> RejectApplication(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var app = await _db.SellerApplications.FindAsync(id);
        if (app == null) return NotFound();
        if (app.Status != "Pending") return BadRequest(new { error = "Only pending applications can be rejected." });

        app.Status = "Rejected";
        app.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { app.Id, app.Status });
    }
}
