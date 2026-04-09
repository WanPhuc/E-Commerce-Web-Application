using Azure.Core;
using Blog.Models.Dto.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebBanHang.Helpers;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Auth;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Services.Global.Implements
{
    public class AuthService:IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository,IConfiguration configuration, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }
        public async Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return ApiResponse<AuthResponseDto>.Fail("Email and password are required", 400);
            }
            var user = await _userRepository.FindSingleByConditionAsync(u => u.Email == dto.Email && !u.DeleteFlg);
            if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid email or password", 401);
            }
            using var transaction = await _userRepository.BeginTransactionAsync();
            try
            {
                var token = _tokenService.GeneratedAccessToken(user);
                var refreshToken = _tokenService.GeneratedRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);
                var authRespone = new AuthResponseDto
                {
                    Me = new MeDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = user.Role.Name
                    },
                    Tokens = new TokenDto
                    {
                        AccessToken = token,
                        RefreshToken = refreshToken,
                        AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 30))

                    }
                };
                transaction.Commit();
                return ApiResponse<AuthResponseDto>.Success(authRespone, "Sign in successful", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<AuthResponseDto>.Fail("An error occurred during sign in: " + ex.Message, 500);
            }
        }
        public async Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.FullName))
            {
                return ApiResponse<AuthResponseDto>.Fail("Full name, email and password are required", 400);
            }
            var existingUser = await _userRepository.FindSingleByConditionAsync(u => u.Email == dto.Email && !u.DeleteFlg);
            if (existingUser != null)
            {
                return ApiResponse<AuthResponseDto>.Fail("Email is already in use", 409);
            }
            var role = await _roleRepository.FindSingleByConditionAsync(r => r.Name == "User");
            if (role == null)
            {
                return ApiResponse<AuthResponseDto>.Fail("System role 'User' not found", 500);
            }
            using var transaction = await _userRepository.BeginTransactionAsync();
            try
            {
                var newUser = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = PasswordHelper.HashPassword(dto.Password),
                    RoleId = role.Id,
                    IsActive = true
                };
                await _userRepository.CreateAsync(newUser);

                var accessToken = _tokenService.GeneratedAccessToken(newUser);
                var refreshToken = _tokenService.GeneratedRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(newUser.Id, refreshToken);
                var response = new AuthResponseDto
                {
                    Me = new MeDto
                    {
                        Id = newUser.Id,
                        FullName = newUser.FullName,
                        Email = newUser.Email,
                        Role = role.Name
                    },
                    Tokens = new TokenDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60))
                    }
                };
                await _userRepository.EndTransactionAsync();
                return ApiResponse<AuthResponseDto>.Success(response, "Đăng ký và đăng nhập thành công!", 201);
            }
            catch (Exception ex)
            {
                await _userRepository.RollbackTransactionAsync();
                return ApiResponse<AuthResponseDto>.Fail("An error occurred during sign up: " + ex.Message, 500);

            }

        }
        public async Task<ApiResponse<object>> SignOutAsync(SignOutDto dto)
        {
            if(dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return ApiResponse<object>.Fail("Refresh token is required", 400);
            }
            var existingToken= await _tokenService.RevokeRefreshTokenAsync(dto.RefreshToken);
            if (!existingToken)
            {
                return ApiResponse<object>.Fail("Invalid refresh token", 400);
            }
            return ApiResponse<object>.Success(null, "Sign out successful", 200);
        }
        public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.AccessToken) || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return ApiResponse<TokenDto>.Fail("Access token and refresh token are required", 400);
            }
            ClaimsPrincipal principal;
            try
            {
                principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

            }
            catch 
            {
                return ApiResponse<TokenDto>.Fail("Invalid access token", 400);
            }
            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if(userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return ApiResponse<TokenDto>.Fail("Invalid access token", 400);
            }
            var user = await _userRepository.FindSingleByConditionAsync(u=>u.Id==userId ,false,u=>u.Role );
            if (user == null) return ApiResponse<TokenDto>.Fail("User not found", 404);
            var tokenHash = dto.RefreshToken.ToSha256Hash();
            var storedToken = await _refreshTokenRepository.FindSingleByConditionAsync(rt => rt.UserID == userId && rt.TokenHash == tokenHash );
            if(storedToken == null) return ApiResponse<TokenDto>.Fail("Invalid refresh token", 400);
            if(storedToken.IsRevoked) return ApiResponse<TokenDto>.Fail("Refresh token has been revoked", 400);
            if(storedToken.ExpiresAt <= DateTime.UtcNow) return ApiResponse<TokenDto>.Fail($"Token expires at {storedToken.ExpiresAt} - UtcNow: {DateTime.UtcNow}", 400);
            using var transaction = await _userRepository.BeginTransactionAsync();
            try
            {
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(storedToken);

                var newAccessToken = _tokenService.GeneratedAccessToken(user);
                var newRefreshToken = _tokenService.GeneratedRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken);

                await _userRepository.EndTransactionAsync();
                return ApiResponse<TokenDto>.Success(new TokenDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 30))
                }, "Token refreshed successfully", 200);
            }catch (Exception ex)
            {
                await _userRepository.RollbackTransactionAsync();
                return ApiResponse<TokenDto>.Fail("An error occurred while refreshing token: " + ex.Message, 500);


            }
    }
}
