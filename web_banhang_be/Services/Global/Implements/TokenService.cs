using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using WebBanHang.Data;
using WebBanHang.Helpers;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Services.Global.Implements
{
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public TokenService(IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _context = context;
        }
        public JwtSecurityToken ParseToken(string tokenString)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
            return token;
        }
        public string GeneratedAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("role", user.Role?.Name ?? "Customer"), 
                new Claim("name", user.FullName?? "")

            };
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
           
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = int.Parse(jwtSettings["ExpirationMinutes"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                Expires = DateTime.UtcNow.AddMinutes(expires),
                SigningCredentials = creds
            };
            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GeneratedRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var rawToken = Convert.ToBase64String(randomBytes);
            return rawToken;
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
        public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var token = new RefreshToken
            {
                UserID = userId,
                TokenHash = refreshToken.ToSha256Hash(),
                ExpiresAt = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays")),
                IsRevoked = false
                    
            };
            await _refreshTokenRepository.CreateAsync(token);
           
        }
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenHash = refreshToken.ToSha256Hash();
            var token = await _refreshTokenRepository.FindSingleByConditionAsync(rt => rt.TokenHash == tokenHash && !rt.DeleteFlg , true);
            if (token is null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow) { return false; }
            token.IsRevoked = true;
            token.DeleteFlg = true;
            token.RevokedAt = DateTime.UtcNow;
            
            await _refreshTokenRepository.UpdateAsync(token);
            return true;
        }
        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            var tokens = await _refreshTokenRepository.FindByConditionAsync(rt =>
                rt.UserID == userId && !rt.DeleteFlg,true);

            if (tokens != null && tokens.Any())
            {
                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                    token.DeleteFlg = true;
                    token.UpdatedAt = DateTime.UtcNow;
                }
                await _refreshTokenRepository.SaveChangesAsync();
            }
        }
        public bool ValidateAccessToken(string accessToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };
            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken && !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        } 
    }
}
