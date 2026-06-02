using Azure;
using Azure.Core;
using Blog.Models.Dto.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebBanHang.Helpers;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Auth;
using WebBanHang.Models.EntityModels;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Services.Global.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }
        public async Task<ApiResponse<MeDto>> GetCurrentUserInfoAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId, u => u.Role);

            if (user == null)
            {
                return ApiResponse<MeDto>.Fail("User not found", 404, ErrorCodes.Auth.UserNotFound);
            }

            var result = new MeDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role?.Name ?? "Guest"
            };

            return ApiResponse<MeDto>.Success(result, "Success", 200, SuccessCodes.Auth.CurrentUserRetrieved);
        }
        public async Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return ApiResponse<AuthResponseDto>.Fail("Email and password are required", 400, ErrorCodes.Auth.MissingCredentials);
            }
            var user = await _userRepository.FindSingleByConditionAsync(u => u.Email == dto.Email && !u.DeleteFlg, false, u => u.Role);
            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid email", 401, ErrorCodes.Auth.InvalidEmail);
            }
            var passwordHash = user.PasswordHash ?? string.Empty;
            bool isValid = false;

            // Bước 1: Thử verify bằng BCrypt (Mới)
            try
            {
                isValid = PasswordHelper.VerifyPassword(dto.Password, passwordHash);
            }
            catch
            {
                // Nếu lỗi (do format hash cũ không phải BCrypt), ta mặc định false để xuống check tiếp
                isValid = false;
            }

            // Bước 2: Nếu BCrypt sai, thử check theo kiểu cũ (Ví dụ SHA256)
            if (!isValid)
            {
                // 2. Thử SHA256 dạng Hex (chuỗi 64 ký tự như a1b2c3...)
                if (PasswordHelper.VerifyOldHash_SHA256(dto.Password, passwordHash))
                {
                    isValid = true;
                }
                // 3. Thử SHA256 dạng Base64 (chuỗi có ký tự đặc biệt như / + = ở cuối)
                else if (PasswordHelper.VerifyOldHash_SHA256_Base64(dto.Password, passwordHash))
                {
                    isValid = true;
                }

                // Nếu đúng bằng một trong hai cách cũ, hãy nâng cấp lên BCrypt
                if (isValid)
                {
                    user.PasswordHash = PasswordHelper.HashPassword(dto.Password);
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                }
            }

            if (!isValid) return ApiResponse<AuthResponseDto>.Fail("Invalid password", 401, ErrorCodes.Auth.InvalidPassword);
            using var transaction = await _userRepository.BeginTransactionAsync();
            try
            {
                await _tokenService.RevokeAllUserTokensAsync(user.Id);

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
                return ApiResponse<AuthResponseDto>.Success(authRespone, "Sign in successful", 200, SuccessCodes.Auth.SignInSuccess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<AuthResponseDto>.Fail("An error occurred during sign in: " + ex.Message, 500, ErrorCodes.Common.InternalServerError);
            }
        }
        public async Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.FullName))
            {
                return ApiResponse<AuthResponseDto>.Fail("Full name, email and password are required", 400, ErrorCodes.Auth.MissingCredentials);
            }
            var existingUser = await _userRepository.FindSingleByConditionAsync(u => u.Email == dto.Email && !u.DeleteFlg);
            if (existingUser != null)
            {
                return ApiResponse<AuthResponseDto>.Fail("Email is already in use", 409, ErrorCodes.Auth.EmailInUse);
            }
            var role = await _roleRepository.FindSingleByConditionAsync(r => r.Name == "Buyer");
            if (role == null)
            {
                return ApiResponse<AuthResponseDto>.Fail("System role 'Buyer' not found", 500, ErrorCodes.Auth.RoleNotFound);
            }
            using var transaction = await _userRepository.BeginTransactionAsync();
            try
            {
                var newUser = new User
                {
                    ProviderType = (byte)BaseEnums.UserProviderTypeEnum.Email,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = PasswordHelper.HashPassword(dto.Password),
                    RoleId = role.Id,
                    IsActive = true
                };
                await _userRepository.CreateAsync(newUser);
                newUser.Role = role;
                var accessToken = _tokenService.GeneratedAccessToken(newUser);
                var refreshToken = _tokenService.GeneratedRefreshToken();
                if (accessToken == null || refreshToken == null)
                {
                    return ApiResponse<AuthResponseDto>.Fail("Failed to generate tokens", 500, ErrorCodes.Common.InternalServerError);
                }
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
                    _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 30))
                    }
                };
                await _userRepository.EndTransactionAsync();
                return ApiResponse<AuthResponseDto>.Success(response, "Đăng ký và đăng nhập thành công!", 201, SuccessCodes.Auth.SignUpSuccess);
            }
            catch (Exception ex)
            {
                await _userRepository.RollbackTransactionAsync();
                return ApiResponse<AuthResponseDto>.Fail("An error occurred during sign up: " + ex.Message, 500, ErrorCodes.Common.InternalServerError);

            }

        }
        public async Task<ApiResponse<object>> SignOutAsync(SignOutDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return ApiResponse<object>.Fail("Refresh token is required", 400, ErrorCodes.Auth.InvalidRefreshToken);
            }
            var existingToken = await _tokenService.RevokeRefreshTokenAsync(dto.RefreshToken);
            if (!existingToken)
            {
                return ApiResponse<object>.Fail("Invalid refresh token", 400, ErrorCodes.Auth.InvalidRefreshToken);
            }
            return ApiResponse<object>.Success(null, "Sign out successful", 200, SuccessCodes.Auth.SignOutSuccess);
        }
        public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return ApiResponse<TokenDto>.Fail("Refresh token is required", 400, ErrorCodes.Auth.InvalidRefreshToken);
            }
            var tokenHash = dto.RefreshToken.ToSha256Hash();
            var storedToken = await _refreshTokenRepository.FindSingleByConditionAsync(rt => rt.TokenHash == tokenHash && !rt.DeleteFlg, true);
            if (storedToken == null) return ApiResponse<TokenDto>.Fail("Invalid refresh token", 400, ErrorCodes.Auth.InvalidRefreshToken);
            if (storedToken.IsRevoked) return ApiResponse<TokenDto>.Fail("Refresh token has been revoked", 400, ErrorCodes.Auth.RefreshTokenRevoked);
            if (storedToken.ExpiresAt <= DateTime.UtcNow) return ApiResponse<TokenDto>.Fail($"Token expires at {storedToken.ExpiresAt} - UtcNow: {DateTime.UtcNow}", 400, ErrorCodes.Auth.RefreshTokenExpired);

            var user = await _userRepository.FindSingleByConditionAsync(u => u.Id == storedToken.UserID && !u.DeleteFlg, false, u => u.Role);
            if (user == null) return ApiResponse<TokenDto>.Fail("User not found", 404, ErrorCodes.Auth.UserNotFound);

            if (!string.IsNullOrWhiteSpace(dto.AccessToken))
            {
                try
                {
                    var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
                    var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                    if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var accessTokenUserId) || accessTokenUserId != storedToken.UserID)
                    {
                        return ApiResponse<TokenDto>.Fail("Invalid access token", 400, ErrorCodes.Auth.InvalidAccessToken);
                    }
                }
                catch
                {
                    return ApiResponse<TokenDto>.Fail("Invalid access token", 400, ErrorCodes.Auth.InvalidAccessToken);
                }
            }

            using var transaction = await _userRepository.BeginTransactionAsync();
            try
            {
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;

                var newAccessToken = _tokenService.GeneratedAccessToken(user);
                var newRefreshToken = _tokenService.GeneratedRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken);

                await _userRepository.EndTransactionAsync();
                return ApiResponse<TokenDto>.Success(new TokenDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 30))
                }, "Token refreshed successfully", 200, SuccessCodes.Auth.TokenRefreshed);
            }
            catch (Exception ex)
            {
                await _userRepository.RollbackTransactionAsync();
                return ApiResponse<TokenDto>.Fail("An error occurred while refreshing token: " + ex.Message, 500, ErrorCodes.Common.InternalServerError);


            }
        }
        public async Task<ApiResponse<AuthResponseDto>> ExternalSignInAsync(ExternalUserInfoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProviderId) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Provider))
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid external user info", 400, ErrorCodes.Auth.ExternalUserInvalid);
            }
            var existingUser = await _userRepository.FindSingleByConditionAsync(u => u.Email == dto.Email && !u.DeleteFlg, false, u => u.Role);
            if (existingUser != null)
            {
                await _tokenService.RevokeAllUserTokensAsync(existingUser.Id);
                var accessToken = _tokenService.GeneratedAccessToken(existingUser);
                var refreshToken = _tokenService.GeneratedRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(existingUser.Id, refreshToken);

                var response = new AuthResponseDto
                {
                    Me = new MeDto
                    {
                        Id = existingUser.Id,
                        FullName = existingUser.FullName,
                        Email = existingUser.Email,
                        Role = existingUser.Role.Name
                    },
                    Tokens = new TokenDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                            _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 30))
                    }
                };
                return ApiResponse<AuthResponseDto>.Success(response, "External sign in successful", 200, SuccessCodes.Auth.ExternalSignInSuccess);
            }
            var role = await _roleRepository.FindSingleByConditionAsync(r => r.Name == "Buyer");
            if (role == null) return ApiResponse<AuthResponseDto>.Fail("Role not found", 404, ErrorCodes.Auth.RoleNotFound);
            using var transaction = await _userRepository.BeginTransactionAsync();
            try
            {
                var newUser = new User
                {
                    ProviderType = dto.Provider.ToLower() switch
                    {
                        "google" => (byte)BaseEnums.UserProviderTypeEnum.Google,
                        "facebook" => (byte)BaseEnums.UserProviderTypeEnum.Facebook,
                        _ => (byte)BaseEnums.UserProviderTypeEnum.Email
                    },
                    ProviderUserId = dto.ProviderId,
                    FullName = dto.Name,
                    Email = dto.Email,
                    RoleId = role.Id,
                    IsActive = true,
                };
                await _userRepository.CreateAsync(newUser);
                newUser.Role = role;
                var accessToken = _tokenService.GeneratedAccessToken(newUser);
                var refreshToken = _tokenService.GeneratedRefreshToken();
                if (accessToken == null || refreshToken == null)
                {
                    return ApiResponse<AuthResponseDto>.Fail("Failed to generate tokens", 500, ErrorCodes.Auth.ExternalLoginFailed);
                }
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
                    _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 30))
                    }
                };
                await _userRepository.EndTransactionAsync();
                return ApiResponse<AuthResponseDto>.Success(response, "External sign in successful", 201, SuccessCodes.Auth.ExternalSignInSuccess);

            }
            catch (Exception ex)
            {
                await _userRepository.RollbackTransactionAsync();
                return ApiResponse<AuthResponseDto>.Fail("An error occurred during external sign in: " + ex.Message, 500, ErrorCodes.Common.InternalServerError);

            }
        }
    }
}
