using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
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
            if(endpoint == null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Endpoint not found",500));
                return;
            }
            if(endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() is object)
            {
                await _next(context);
                return;
            }
            else
            {
                var token = context.Request.Cookies["accessToken"] ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if(string.IsNullOrEmpty(token))
                {
                    await WriteUnauthorized(context);
                    return;
                }
                var isValidToken= tokenService.ValidateAccessToken(token);
                if (isValidToken)
                {
                    //parse token to get claims
                    var jwtSecurityToken = tokenService.ParseToken(token);
                    // get user id from claims
                    var userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                    if (!Guid.TryParse(userIdClaim, out Guid userId))
                    {
                        await WriteUnauthorized(context);
                        return;
                    }
                    var isUserExist = await userRepository.AnyByConditionAsync(u => u.Id == userId && !u.DeleteFlg);
                    if (!isUserExist)
                    {
                        await WriteUnauthorized(context);
                        return;
                    }
                    commonService.SetUserId(userId);
                }
                else
                {
                    await WriteUnauthorized(context);
                    return;
                }
            }
        }
        public async Task WriteUnauthorized(HttpContext context)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Unauthorized", 401));
        }
    }
}
