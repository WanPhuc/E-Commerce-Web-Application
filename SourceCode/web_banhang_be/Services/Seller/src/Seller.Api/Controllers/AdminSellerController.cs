using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuraMart.Seller.Infrastructure.Persistence;

namespace AuraMart.Seller.Api.Controllers;

/// <summary>
/// Admin endpoints for seller management.
/// Route prefix: /api/v1/admin/sellers
/// Mapped via YARP: /api/v1/admin/sellers/{**catch-all} → seller cluster
/// </summary>
[ApiController]
[Route("api/v1/admin/sellers")]
[Authorize(Roles = "Admin")]
public class AdminSellerController : BaseController
{
    private readonly SellerDbContext _db;

    public AdminSellerController(SellerDbContext db)
    {
        _db = db;
    }

    // GET /api/v1/admin/sellers
    // Trả về danh sách seller + pending applications để admin quản lý
    [HttpGet]
    public async Task<IActionResult> GetSellerManagement()
    {
        var sellers = await _db.Sellers
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new
            {
                s.Id,
                s.StoreName,
                s.Description,
                s.UserId,
                s.Status,
                s.CreatedAt
            })
            .ToListAsync();

        var pendingApplications = await _db.SellerApplications
            .AsNoTracking()
            .Where(a => a.Status == "Pending")
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.UserId,
                a.ShopName,
                a.Description,
                a.PhoneNumber,
                a.City,
                a.District,
                a.Ward,
                a.AddressLine,
                Status = a.Status,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            sellers,
            pendingApplications,
            totalSellers = sellers.Count,
            totalPending = pendingApplications.Count
        }));
    }

    // GET /api/v1/admin/sellers/sellers/{id}
    // Chi tiết seller
    [HttpGet("sellers/{id:guid}")]
    public async Task<IActionResult> GetSellerDetail(Guid id)
    {
        var seller = await _db.Sellers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (seller == null)
            return NotFound(ApiResponse<object>.Fail("Không tìm thấy seller", 404));

        return Ok(ApiResponse<object>.Success(new
        {
            seller.Id,
            seller.StoreName,
            seller.Description,
            seller.UserId,
            seller.AddressId,
            Status = seller.Status,
            seller.CreatedAt
        }));
    }

    // GET /api/v1/admin/sellers/application-seller/{id}
    // Chi tiết đơn đăng ký seller
    [HttpGet("application-seller/{id:guid}")]
    public async Task<IActionResult> GetSellerApplicationDetail(Guid id)
    {
        var app = await _db.SellerApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null)
            return NotFound(ApiResponse<object>.Fail("Không tìm thấy đơn đăng ký", 404));

        return Ok(ApiResponse<object>.Success(new
        {
            app.Id,
            app.UserId,
            app.ShopName,
            app.Description,
            app.PhoneNumber,
            app.City,
            app.District,
            app.Ward,
            app.AddressLine,
            Status = app.Status,
            app.CreatedAt,
            app.ReviewedAt
        }));
    }

    // POST /api/v1/admin/sellers/{id}/approved
    // Duyệt đơn đăng ký seller
    [HttpPost("{id:guid}/approved")]
    public async Task<IActionResult> ApproveSeller(Guid id)
    {
        var app = await _db.SellerApplications.FindAsync(id);
        if (app == null)
            return NotFound(ApiResponse<object>.Fail("Không tìm thấy đơn đăng ký", 404));

        if (app.Status != "Pending")
            return BadRequest(ApiResponse<object>.Fail("Chỉ có thể duyệt đơn ở trạng thái Pending", 400));

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

        return Ok(ApiResponse<object>.Success(new { seller.Id, seller.StoreName }, "Duyệt đơn thành công"));
    }

    // POST /api/v1/admin/sellers/{id}/rejected
    // Từ chối đơn đăng ký seller
    [HttpPost("{id:guid}/rejected")]
    public async Task<IActionResult> RejectSeller(Guid id)
    {
        var app = await _db.SellerApplications.FindAsync(id);
        if (app == null)
            return NotFound(ApiResponse<object>.Fail("Không tìm thấy đơn đăng ký", 404));

        if (app.Status != "Pending")
            return BadRequest(ApiResponse<object>.Fail("Chỉ có thể từ chối đơn ở trạng thái Pending", 400));

        app.Status = "Rejected";
        app.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new { app.Id, app.Status }, "Từ chối đơn thành công"));
    }
}
