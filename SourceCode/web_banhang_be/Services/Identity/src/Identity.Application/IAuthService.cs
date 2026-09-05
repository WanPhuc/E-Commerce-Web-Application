


using AuraMart.Identity.Dtos;
using AuraMart.Identity.Domain.Repositories;
using AuraMart.Identity.Contracts;
namespace AuraMart.Identity.Application;
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto dto);
        Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto dto);
        Task<ApiResponse<TokenDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task<ApiResponse<object>> SignOutAsync(SignOutDto dto);
        Task<ApiResponse<MeDto>> GetCurrentUserInfoAsync(Guid userId);
        //Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto dto);
        //Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequestDto dto);
        Task<ApiResponse<AuthResponseDto>> ExternalSignInAsync(ExternalUserInfoDto dto);

    }
