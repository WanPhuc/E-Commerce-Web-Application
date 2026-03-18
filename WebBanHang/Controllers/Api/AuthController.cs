using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Helpers;
using WebBanHang.Models;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Auth;
using WebBanHang.Models.ViewModels;

namespace WebBanHang.Controllers.Api;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("signin")]
    public async Task<ActionResult<ApiResponse<MeDto>>> SignIn([FromBody] LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<MeDto>
            {
                Status = 400,
                Message = "Dữ liệu không hợp lệ.",
                Data = null!
            });
        }

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == viewModel.Email && u.IsActive);

        if (user == null || !PasswordHelper.VerifyPassword(viewModel.Password, user.PasswordHash))
        {
            return Unauthorized(new ApiResponse<MeDto>
            {
                Status = 401,
                Message = "Email hoặc mật khẩu không đúng.",
                Data = null!
            });
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Name),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        var me = new MeDto
        {
            Id = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name
        };

        return Ok(new ApiResponse<MeDto>
        {
            Status = 200,
            Message = "Đăng nhập thành công.",
            Data = me
        });
    }

    [HttpGet("me")]
    public ActionResult<ApiResponse<MeDto>> Me()
    {
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Unauthorized(new ApiResponse<MeDto>
            {
                Status = 401,
                Message = "Chưa đăng nhập.",
                Data = null!
            });
        }

        var me = new MeDto
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            FullName = User.FindFirstValue(ClaimTypes.Name) ?? "",
            Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
            Role = User.FindFirstValue(ClaimTypes.Role) ?? ""
        };

        return Ok(new ApiResponse<MeDto>
        {
            Status = 200,
            Message = "Success",
            Data = me
        });
    }

    [HttpPost("signout")]
    public async Task<ActionResult<ApiResponse<object?>>> SignOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok(new ApiResponse<object?>
        {
            Status = 200,
            Message = "Đăng xuất thành công.",
            Data = null
        });
    }

    [HttpPost("signup")]
    public async Task<ActionResult<ApiResponse<MeDto>>> SignUp([FromBody] SignupViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(", ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return BadRequest(new ApiResponse<MeDto>
            {
                Status = 400,
                Message = errors,
                Data = null!
            });
        }

        var emailExists = await _db.Users.AnyAsync(e => e.Email == viewModel.Email);
        if (emailExists)
        {
            return Conflict(new ApiResponse<MeDto>
            {
                Status = 409,
                Message = "Email đã được sử dụng.",
                Data = null!
            });
        }

        var defaultRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (defaultRole == null)
        {
            return StatusCode(500, new ApiResponse<MeDto>
            {
                Status = 500,
                Message = "Role User chưa được khởi tạo.",
                Data = null!
            });
        }

        var user = new User
        {
            FullName = viewModel.FullName,
            Email = viewModel.Email,
            PasswordHash = PasswordHelper.HashPassword(viewModel.Password),
            CreatedAt = DateTime.UtcNow,
            RoleId = defaultRole.Id,
            IsActive = true
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        // Login luôn sau khi signup (giữ đúng như code cũ của m)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, "User"),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var me = new MeDto
        {
            Id = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email,
            Role = "User"
        };

        return Ok(new ApiResponse<MeDto>
        {
            Status = 200,
            Message = "Đăng ký thành công.",
            Data = me
        });
    }
}
