using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebBanHang.Models.DTOs.Auth;
using WebBanHang.Models.EntityModels;

namespace WebBanHang.Services.Global.Interfaces
{
    public interface ITokenService
    {
        string GeneratedAccessToken(User user);
        string GeneratedRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task SaveRefreshTokenAsync(Guid userId, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAllUserTokensAsync(Guid userId);
        bool ValidateAccessToken(string token);
        JwtSecurityToken ParseToken(string token);
        
    }
}
