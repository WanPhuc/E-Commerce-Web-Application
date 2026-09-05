using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

using AuraMart.Identity.Dtos;


using AuraMart.Identity.Contracts;
namespace AuraMart.Identity.Application;
using AuraMart.Identity.Domain.Repositories;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Phase 6.1: thong tin seller nam o CORE app - goi qua internal API
    private async Task<Dictionary<Guid, (Guid Id, string StoreName, string Status)>> GetSellersByUserIdsAsync(List<Guid> userIds)
    {
        var result = new Dictionary<Guid, (Guid, string, string)>();
        try
        {
            var client = _httpClientFactory.CreateClient("core");
            client.DefaultRequestHeaders.Add("X-Internal-Api-Key", _configuration["InternalApi:Key"] ?? "dev-internal-key");
            var idsQuery = string.Join(",", userIds);
            var response = await client.GetFromJsonAsync<List<SellerInfoDto>>($"/internal/sellers/by-user-ids?ids={idsQuery}");
            foreach (var s in response ?? new())
                result[s.userId] = (s.id, s.storeName ?? "", s.status ?? "");
        }
        catch { /* seller info la phu - khong chan danh sach user */ }
        return result;
    }
    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllUserAsync(PagedRequest request)
    {
        var users = _userRepository.FindByCondition(
            u => !u.DeleteFlg,
            false,
            u => u.Role);

        var resultQuery = users
            .OrderByDescending(user => user.CreatedAt)
            .Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role != null ? user.Role.Name : "Unknown"
            });

        var result = await resultQuery.ToPagedAsync(request);

        // Phase 6.1: thong tin seller nam o core app - lay qua internal API
        if (result.Data.Count > 0)
        {
            var userIds = result.Data.Select(u => u.Id).ToList();
            var sellerMap = await GetSellersByUserIdsAsync(userIds);
            foreach (var item in result.Data)
            {
                if (sellerMap.TryGetValue(item.Id, out var s))
                {
                    item.Seller = new SellerSummaryDto
                    {
                        Id = s.Item1,
                        ShopName = s.Item2,
                        Status = Enum.TryParse<SellerApplicationStatus>(s.Item3, out var st) ? st : default
                    };
                }
            }
        }

        return ApiResponse<PagedResult<UserDto>>.Success(result, "Success", 200, SuccessCodes.User.ListRetrieved);

    }
    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId, u => u.Role);
        if (user == null)
        {
            return ApiResponse<UserDto>.Fail("User not found.", 404, ErrorCodes.User.NotFound);
        }
        var singleMap = await GetSellersByUserIdsAsync(new List<Guid> { userId });
        SellerSummaryDto? MapSeller()
        {
            if (!singleMap.TryGetValue(userId, out var s)) return null;
            return new SellerSummaryDto
            {
                Id = s.Item1,
                ShopName = s.Item2,
                Status = Enum.TryParse<SellerApplicationStatus>(s.Item3, out var st2) ? st2 : default
            };
        }
        var seller = MapSeller();
        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            Role = user.Role?.Name ?? "Unknown",
            Seller = seller
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

public record SellerInfoDto(Guid userId, Guid id, string? storeName, string? status);
