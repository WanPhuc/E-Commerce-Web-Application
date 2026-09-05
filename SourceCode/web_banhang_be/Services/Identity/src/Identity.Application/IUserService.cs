
using AuraMart.Identity.Dtos;
using AuraMart.Identity.Domain.Repositories;
using AuraMart.Identity.Contracts;
namespace AuraMart.Identity.Application;
public interface IUserService
{
    Task<ApiResponse<PagedResult<UserDto>>> GetAllUserAsync(PagedRequest request);
    Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateuserDto createuserDto);
}
