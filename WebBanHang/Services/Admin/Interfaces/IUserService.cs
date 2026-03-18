using WebBanHang.Models.DTOs.Users;

namespace WebBanHang.Services.Interfaces;
public interface IUserService
{
    Task<List<UserDto>> GetAllUserAsync();
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<UserDto> CreateUserAsync(CreateuserDto createuserDto);
}