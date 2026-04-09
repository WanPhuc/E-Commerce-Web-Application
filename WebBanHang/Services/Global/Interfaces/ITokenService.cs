using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        bool ValidateAccessToken(string token);
        JwtSecurityToken ParseToken(string token);
    }
}
