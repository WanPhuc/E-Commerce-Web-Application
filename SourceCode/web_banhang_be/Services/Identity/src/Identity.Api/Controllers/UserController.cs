
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanFucVN.Core.Web.Controllers;


using AuraMart.Identity.Dtos;
using AuraMart.Identity.Application;
using AuraMart.Identity.Domain.Repositories;
namespace AuraMart.Identity.Controllers;
[ApiController]
[Route("api/v1/admin/users")]
[AuthorizeRole("Admin")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] PagedRequest request)
    {   
        var users = await _userService.GetAllUserAsync(request);
        return BaseResult(users);
    }
        
    [HttpGet("detail/{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return BaseResult(user);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateuserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return BaseResult(user);
    }
}
