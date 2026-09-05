using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VanFucVN.Core.Common.Constants;
using VanFucVN.Core.Common.DTOs;
using VanFucVN.Core.Common.Services;

namespace VanFucVN.Core.Web.Middlewares;

public class CustomAuthorizeMiddleware
{
    private readonly RequestDelegate _next;

    public CustomAuthorizeMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, ICommonService commonService, IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache cache)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        if (endpoint.Metadata?.GetMetadata<AllowAnonymousAttribute>() is not null)
        {
            await _next(context);
            return;
        }

        var token = context.Request.Cookies["accessToken"]
                    ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            await WriteUnauthorized(context);
            return;
        }

        var principal = ValidateTokenLocally(token, configuration);
        if (principal == null)
        {
            await WriteUnauthorized(context);
            return;
        }
        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
                       ?? principal.FindFirst(ClaimTypes.NameIdentifier)
                       ?? principal.FindFirst("userId");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var parsedUserId))
        {
            await WriteUnauthorized(context);
            return;
        }

        context.User = principal;

        if (!await IsUserAliveAsync(parsedUserId, httpClientFactory, configuration, cache))
        {
            await WriteUnauthorized(context);
            return;
        }

        commonService.SetUserId(parsedUserId);
        await _next(context);
    }

    private static ClaimsPrincipal? ValidateTokenLocally(string token, IConfiguration configuration)
    {
        try
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true
            }, out var validated);

            if (validated is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private static async Task<bool> IsUserAliveAsync(
        Guid userId, IHttpClientFactory factory, IConfiguration configuration, IMemoryCache cache)
    {
        var cacheKey = $"user-alive:{userId}";
        if (cache.TryGetValue(cacheKey, out bool alive)) return alive;

        try
        {
            var client = factory.CreateClient("identity");
            client.DefaultRequestHeaders.Add("X-Internal-Api-Key", configuration["InternalApi:Key"] ?? "dev-internal-key");
            var response = await client.GetAsync($"/internal/users/{userId}/exists");
            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadFromJsonAsync<ExistsResponse>();
            alive = payload?.exists ?? false;
        }
        catch
        {
            alive = false;
        }

        cache.Set(cacheKey, alive, TimeSpan.FromSeconds(60));
        return alive;
    }

    private record ExistsResponse(bool exists);

    private static async Task WriteUnauthorized(HttpContext context)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Unauthorized", 401, ErrorCodes.Common.Unauthorized));
    }
}
