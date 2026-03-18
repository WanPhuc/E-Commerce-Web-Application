using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Users;
using WebBanHang.Models.ViewModels;
using WebBanHang.Services.Interfaces;
namespace WebBanHang.Controllers.Api.Admin;
[ApiController]
[Route("api/v1/admin/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
    {   
        var users = await _userService.GetAllUserAsync();
        return Ok(ApiResponse<List<UserDto>>.Ok(users));
    }
        
    [HttpGet("detail/{id:guid}")]
    public async Task<ActionResult<ApiResponse<Object?>>> GetUserById(Guid id)
    {
        try{
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(new ApiResponse<Object?>
            {
                Status = 200,
                Message = "Success",
                Data = user
            });
                
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object?>.Fail(ex.Message, 404));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object?>.Fail("Internal server error", 500));
        }

    }
    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateuserDto dto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(dto);
            return Ok(ApiResponse<UserDto>.Ok(user,"User created successfully"));
        }catch(InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserDto>.Fail(ex.Message,400));
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UserDto>.Fail(ex.Message,404));
        }
        catch(Exception ex)
        {
            return StatusCode(500,ApiResponse<UserDto>.Fail(ex.Message,500));
        }
    }
}
