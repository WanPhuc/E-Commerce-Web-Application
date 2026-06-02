using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebBanHang.Models.Common;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Middleware
{
    public class CustomAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomAuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, ITokenService tokenService, ICommonService commonService, IUserRepository userRepository)
        {
            var endpoint = context.GetEndpoint();

            // 1. Nếu không tìm thấy endpoint, cứ để nó đi tiếp để hệ thống xử lý 404
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // 2. Kiểm tra AllowAnonymous
            if (endpoint.Metadata?.GetMetadata<AllowAnonymousAttribute>() is not null)
            {
                await _next(context);
                return;
            }

            // 3. Lấy và kiểm tra Token
            var token = context.Request.Cookies["accessToken"]
                        ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                await WriteUnauthorized(context);
                return;
            }

            var isValidToken = tokenService.ValidateAccessToken(token);
            if (isValidToken)
            {
                var jwtSecurityToken = tokenService.ParseToken(token);
                var userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    var isUserExist = await userRepository.AnyByConditionAsync(u => u.Id == userId && !u.DeleteFlg);
                    if (isUserExist)
                    {
                        // Set UserId cho logic nghiệp vụ
                        commonService.SetUserId(userId);

                        // Gán identity cho hệ thống bảo mật của .NET
                        var identity = new ClaimsIdentity(jwtSecurityToken.Claims, "JwtAuth", JwtRegisteredClaimNames.Sub, "role");
                        context.User = new ClaimsPrincipal(identity);

                        // Đi tiếp vào Controller và thoát khỏi Middleware này
                        await _next(context);
                        return;
                    }
                }
            }

            // Nếu không khớp điều kiện nào ở trên (token sai, user ko tồn tại...)
            await WriteUnauthorized(context);
        }
        public async Task WriteUnauthorized(HttpContext context)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Unauthorized", 401, ErrorCodes.Common.Unauthorized));
        }
    }
}
