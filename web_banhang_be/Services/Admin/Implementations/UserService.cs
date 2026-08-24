using Microsoft.AspNetCore.Identity;
using WebBanHang.Extensions;
using WebBanHang.Helpers;
using WebBanHang.Models;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.DTOs.Users;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories;
using WebBanHang.Services.Interfaces;

namespace WebBanHang.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }
    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllUserAsync(PagedRequest request)
    {
        var users = _userRepository.FindByCondition(
            u => !u.DeleteFlg,
            false,
            u => u.Role,
            u => u.Seller!);

        var resultQuery = users
            .OrderByDescending(user => user.CreatedAt)
            .Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role != null ? user.Role.Name : "Unknown",
                Seller = user.Seller != null ? new SellerSummaryDto
                {
                    Id = user.Seller.Id,
                    ShopName = user.Seller.StoreName,
                    Status = user.Seller.Status
                } : null
            });

        var result = await resultQuery.ToPagedAsync(request);
        return ApiResponse<PagedResult<UserDto>>.Success(result, "Success", 200, SuccessCodes.User.ListRetrieved);

    }
    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId, u => u.Role, u => u.Seller!);
        if (user == null)
        {
            return ApiResponse<UserDto>.Fail("User not found.", 404, ErrorCodes.User.NotFound);
        }
        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            Role = user.Role?.Name ?? "Unknown",
            Seller = user.Seller != null ? new SellerSummaryDto
            {
                Id = user.Seller.Id,
                ShopName = user.Seller.StoreName,
                Status = user.Seller.Status
            } : null
        };
        return ApiResponse<UserDto>.Success(userDto, "Success", 200, SuccessCodes.User.Retrieved);
    }
    public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateuserDto createuserDto)
    {
        var emailExists = await _userRepository.AnyByConditionAsync(e => e.Email == createuserDto.Email);
        if (emailExists)
        {
            return ApiResponse<UserDto>.Fail("Email already in use.", 400, ErrorCodes.User.EmailInUse);
        }
        var role = await _roleRepository.FindSingleByConditionAsync(r => r.Name == "User");
        if (role == null)
        {
            return ApiResponse<UserDto>.Fail("Default role not found.", 404, ErrorCodes.User.DefaultRoleNotFound);
        }
        var passwordHash = PasswordHelper.HashPassword(createuserDto.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = createuserDto.FullName,
            Email = createuserDto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            RoleId = role.Id,

        };
        await _userRepository.CreateAsync(user);

        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            Role = role.Name,
            Seller = null
        };
        return ApiResponse<UserDto>.Success(userDto, "User created successfully", 201, SuccessCodes.User.Created);
    }
}
