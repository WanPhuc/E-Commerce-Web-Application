using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Users;

namespace WebBanHang.Services.Interfaces;
public interface IUserService
{
    Task<ApiResponse<PagedResult<UserDto>>> GetAllUserAsync(PagedRequest request);
    Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateuserDto createuserDto);
}
