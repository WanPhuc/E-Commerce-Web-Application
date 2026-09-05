
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebBanHang.Data;
using AuraMart.Identity.Dtos;

using BuildingBlocks.Redis;
using VanFucVN.Core.Common.Services;
using VanFucVN.Core.Common.DTOs;
using VanFucVN.Core.Common.Constants;
using VanFucVN.Core.Web.Controllers;
using AuraMart.Identity.Application;
using AuraMart.Identity.Domain.Repositories;
namespace AuraMart.Identity.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly ICommonService _commonService;
    private readonly ICacheService _cacheService;
    public AuthController(IAuthService authService, IConfiguration configuration, ICommonService commonService, ICacheService cacheService)
    {
        _authService = authService;
        _configuration = configuration;
        _commonService = commonService;
        _cacheService = cacheService;
    }
    [HttpPost("signin")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
    {
        var result = await _authService.SignInAsync(dto);
        if (result.Status < 200 || result.Status >= 300) return BaseResult(result);
        SetTokenCookies(
            result.Data!.Tokens.AccessToken,
            result.Data.Tokens.RefreshToken,
            result.Data.Tokens.AccessTokenExpiresAt);
        return BaseResult(result);
    }
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto dto)
    {
        var result = await _authService.SignUpAsync(dto);
        if (result.Status < 200 || result.Status >= 300) return BaseResult(result);
        SetTokenCookies(
            result.Data!.Tokens.AccessToken,
            result.Data.Tokens.RefreshToken,
            result.Data.Tokens.AccessTokenExpiresAt);
        return BaseResult(result);
    }
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        var accessToken = Request.Cookies["accessToken"];
        var refreshToken = Request.Cookies["refreshToken"];
        var result = await _authService.RefreshTokenAsync(new RefreshTokenRequestDto
        {
            AccessToken = accessToken!,
            RefreshToken = refreshToken!
        });
        if (result.Status != 200) return BaseResult(result);
        SetTokenCookies(
            result.Data!.AccessToken,
            result.Data.RefreshToken,
            result.Data.AccessTokenExpiresAt);
        return BaseResult(result);
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BaseResult(ApiResponse.Fail("Token not found", 400, ErrorCodes.Auth.TokenNotFound));

        var result = await _authService.SignOutAsync(new SignOutDto
        {
            RefreshToken = refreshToken
        });
        if (result.Status != 200) return BaseResult(result);
        ClearTokenCookies();
        return BaseResult(result);
    }
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        // Lấy UserId đã được Middleware của bạn set vào CommonService
        var userId = _commonService.GetUserId();

        var result = await _authService.GetCurrentUserInfoAsync(userId);

        return BaseResult(result);
    }

    //login O2Auth
    [AllowAnonymous]
    [HttpGet("external-login")]
    public IActionResult ExternalLogin([FromQuery] string provider)
    {
        if (string.IsNullOrWhiteSpace(provider)) return BaseResult(ApiResponse.Fail("Provider is required", 400, ErrorCodes.Auth.ProviderRequired));
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("ExternalLoginCallback", "Auth", new { provider })
        };
        return Challenge(properties, provider);
    }
    [AllowAnonymous]
    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback([FromQuery] string provider)
    {
        // 1. Lấy thông tin từ "chiếc hộp" Cookie mà Google/Facebook vừa gửi về
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
            return BaseResult(ApiResponse.Fail($"{provider} login failed", 401, ErrorCodes.Auth.LoginFailed));

        // 2. Bóc tách thông tin (Email, Name, ID)
        var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
        var providerUserId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(providerUserId)) return BaseResult(ApiResponse.Fail("Invalid provider data", 400, ErrorCodes.Auth.ProviderDataInvalid));

        var apiResponse = await _authService.ExternalSignInAsync(new ExternalUserInfoDto
        {
            ProviderId = providerUserId ?? "",
            Email = email ?? "",
            Name = name ?? "",
            Provider = provider
        });

        //tạo key va luu vao cache 
        Console.WriteLine($"=== ExternalSignIn Status: {apiResponse.Status}");
        Console.WriteLine($"=== ExternalSignIn Data null: {apiResponse.Data == null}");
        Console.WriteLine($"=== ExternalSignIn Message: {apiResponse.Message}");

        var key = Guid.NewGuid().ToString();
        _cacheService.SetCache(key, apiResponse, 60);

        // 👇 Thêm log để xem key
        Console.WriteLine($"=== Key saved: {key}");

        // 5. Điều hướng về trang Angular kèm theo KEY
        return Redirect($"{_configuration["BaseUrl"]}/auth-callback?key={key}");

    }
    [AllowAnonymous]
    [HttpGet("finalize-login")]
    public async Task<IActionResult> FinalizeLogin([FromQuery] string key)
    {
        if (string.IsNullOrEmpty(key)) return BaseResult(ApiResponse.Fail("Key is required", 400, ErrorCodes.Auth.LoginFailed));
        var result = _cacheService.GetCache<ApiResponse<AuthResponseDto>>(key);
        if (result == null || result.Data == null) return BaseResult(ApiResponse.Fail("Invalid or expired key", 400, ErrorCodes.Auth.LoginFailed));

        SetTokenCookies(
            result.Data.Tokens.AccessToken,
            result.Data.Tokens.RefreshToken,
            result.Data.Tokens.AccessTokenExpiresAt
            );
        _cacheService.RemoveCache(key);
        return BaseResult(ApiResponse<MeDto>.Success(result.Data.Me, "Login success", 200, SuccessCodes.Auth.LoginFinalized));

    }

    //========== Private Helpers ==========
    private void SetTokenCookies(string accessToken, string refreshToken, DateTime expiresAt)
    {
        Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expiresAt
        });
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays", 7))
        });
    }
    private void ClearTokenCookies()
    {
        var options = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None
        };
        Response.Cookies.Delete("accessToken", options);
        Response.Cookies.Delete("refreshToken", options);
    }
}
