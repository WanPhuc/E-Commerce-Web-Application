using Blog.Models.Dto.Auth;
using TearExo.API.User.Models.DtoModels.Auth;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Auth;

namespace WebBanHang.Services.Global.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto dto);
        Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto dto);
        Task<ApiResponse<TokenDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task<ApiResponse<object>> SignOut(SignOutDto dto);
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto dto);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequestDto dto);

    }
}
        