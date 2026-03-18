using Microsoft.AspNetCore.Identity;
using WebBanHang.Helpers;
using WebBanHang.Migrations;
using WebBanHang.Models;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.DTOs.Users;
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
    public async Task<List<UserDto>> GetAllUserAsync()
    {
        var users = await _userRepository.GetAllUserAsync();
        if(users == null)
        {
            throw new KeyNotFoundException("No users found.");
        }
        var userDtos = users.Select(user => new UserDto
        {
            Id =user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            Role = user.Role.Name,
            Seller=user.Seller !=null ? new SellerSummaryDto
            {
                Id=user.Seller.Id,
                ShopName=user.Seller.StoreName,
                Status=user.Seller.Status
            }:null
        }).ToList();

        return userDtos;
    
    }
    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdWithRoleSellerAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        var userDto = new UserDto
        {
            Id =user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            Role = user.Role?.Name??"Unknown",
            Seller=user.Seller !=null ? new SellerSummaryDto
            {
                Id=user.Seller.Id,
                ShopName=user.Seller.StoreName,
                Status=user.Seller.Status
            }:null
        };
        return userDto;       
    }
    public async Task<UserDto> CreateUserAsync(CreateuserDto createuserDto)
    {
        var emailExists = await _userRepository.ExistsByEmailAsync(createuserDto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException("Email already in use.");
        }
        var role =await _roleRepository.GetByNameAsync("User");
        if(role == null)
        {
            throw new KeyNotFoundException("Default role not found.");
        }
        var passwordHash = PasswordHelper.HashPassword(createuserDto.Password);
        var user =new User
        {
            Id=Guid.NewGuid(),
            FullName=createuserDto.FullName,
            Email=createuserDto.Email,
            PasswordHash=passwordHash,
            CreatedAt=DateTime.UtcNow,
            IsActive=true,
            RoleId=role.Id,

        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserDto
        {
            Id=user.Id,
            FullName=user.FullName,
            Email=user.Email,
            CreatedAt=user.CreatedAt,
            IsActive=user.IsActive,
            Role=role.Name,
            Seller=null
        };
    }
}