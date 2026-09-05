using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AuraMart.Identity.Infrastructure.Persistence;
using AuraMart.Identity.Contracts;

namespace AuraMart.Identity.Controllers;

// ============================================================================
// PHASE 6.1 - Internal API: cac service khac (core app) goi qua HTTP thay
// cho viec query truc tiep bang identity.
//
// Bao mat service-to-service: bat buoc header X-Internal-Api-Key.
// Day la "internal auth" da ghi trong plan Phase 4 - khong dung JWT user.
// ============================================================================
[ApiController]
[AllowAnonymous]
[Route("internal")]
public class InternalUserController : ControllerBase
{
    public static string InternalApiKey { get; set; } = "dev-internal-key";

    private readonly IdentityDbContext _db;
    public InternalUserController(IdentityDbContext db) => _db = db;

    private bool IsAuthorized() =>
        Request.Headers.TryGetValue("X-Internal-Api-Key", out var key) && key == InternalApiKey;

    private IActionResult Denied() => StatusCode(403, ApiResponse<object>.Fail("Forbidden", 403, ErrorCodes.Common.Forbidden));

    // GET internal/users/{id}/exists - cho auth middleware cua core (cache 60s o phia core)
    [HttpGet("users/{id:guid}/exists")]
    public async Task<IActionResult> Exists(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var exists = await _db.Users.AsNoTracking().AnyAsync(u => u.Id == id && !u.DeleteFlg);
        return Ok(new { exists });
    }

    // GET internal/users/{id}
    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound();
        return Ok(new UserSnapshot(u.Id, u.FullName, u.Email, u.IsActive));
    }

    // GET internal/users?ids=a,b,c
    [HttpGet("users")]
    public async Task<IActionResult> GetByIds([FromQuery] string ids)
    {
        if (!IsAuthorized()) return Denied();
        var list = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty).ToList();
        var users = await _db.Users.AsNoTracking().Where(u => list.Contains(u.Id)).ToListAsync();
        return Ok(users.Select(u => new UserSnapshot(u.Id, u.FullName, u.Email, u.IsActive)).ToList());
    }

    // GET internal/users/by-email/{email} - cho demo seeder cua core
    [HttpGet("users/by-email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        if (!IsAuthorized()) return Denied();
        var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        if (u == null) return NotFound();
        return Ok(new UserSnapshot(u.Id, u.FullName, u.Email, u.IsActive));
    }

    // GET internal/users/stats/registered-before?utc=...
    [HttpGet("users/stats/registered-before")]
    public async Task<IActionResult> CountRegisteredBefore([FromQuery] DateTime utc)
    {
        if (!IsAuthorized()) return Denied();
        var count = await _db.Users.AsNoTracking()
            .CountAsync(u => u.CreatedAt < utc && (u.Role.Name == "User" || u.Role.Name == "Seller"));
        return Ok(new { count });
    }

    // GET internal/users/stats/registration-dates?from=&to=
    [HttpGet("users/stats/registration-dates")]
    public async Task<IActionResult> RegistrationDates([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (!IsAuthorized()) return Denied();
        var dates = await _db.Users.AsNoTracking()
            .Where(u => u.CreatedAt >= from && u.CreatedAt < to && (u.Role.Name == "User" || u.Role.Name == "Seller"))
            .Select(u => u.CreatedAt).ToListAsync();
        return Ok(dates);
    }

    // GET internal/users/stats/total
    [HttpGet("users/stats/total")]
    public async Task<IActionResult> TotalUsers()
    {
        if (!IsAuthorized()) return Denied();
        var count = await _db.Users.AsNoTracking().CountAsync();
        return Ok(new { count });
    }

    // GET internal/addresses/{id}
    [HttpGet("addresses/{id:guid}")]
    public async Task<IActionResult> GetAddress(Guid id)
    {
        if (!IsAuthorized()) return Denied();
        var a = await _db.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();
        return Ok(new AddressSnapshot(a.Id, a.UserId, a.RecipientName, a.PhoneNumber, a.AddressLine, a.Ward, a.District, a.City, a.IsDefault));
    }

    // POST internal/addresses
    [HttpPost("addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressSnapshot data)
    {
        if (!IsAuthorized()) return Denied();
        var addr = new AuraMart.Identity.Domain.Address
        {
            UserId = data.UserId,
            RecipientName = data.RecipientName,
            PhoneNumber = data.PhoneNumber,
            AddressLine = data.AddressLine,
            Ward = data.Ward,
            District = data.District,
            City = data.City,
            IsDefault = data.IsDefault,
            CreatedAt = DateTime.UtcNow
        };
        _db.Addresses.Add(addr);
        await _db.SaveChangesAsync();
        return Ok(addr.Id);
    }

    // PUT internal/addresses/{id}
    [HttpPut("addresses/{id:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressSnapshot data)
    {
        if (!IsAuthorized()) return Denied();
        var addr = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id);
        if (addr == null) return NotFound();
        addr.RecipientName = data.RecipientName;
        addr.PhoneNumber = data.PhoneNumber;
        addr.AddressLine = data.AddressLine;
        addr.Ward = data.Ward;
        addr.District = data.District;
        addr.City = data.City;
        await _db.SaveChangesAsync();
        return Ok(true);
    }

    // PUT internal/users/{id}/profile
    [HttpPut("users/{id:guid}/profile")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] ProfileUpdateRequest req)
    {
        if (!IsAuthorized()) return Denied();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        user.FullName = req.FullName;
        user.Email = req.Email;
        await _db.SaveChangesAsync();
        return Ok(true);
    }

    public record ProfileUpdateRequest(string FullName, string Email);
}
