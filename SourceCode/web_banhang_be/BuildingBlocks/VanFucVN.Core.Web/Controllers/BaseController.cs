using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using VanFucVN.Core.Common.DTOs;

namespace VanFucVN.Core.Web.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    [NonAction]
    public IActionResult BaseResult<T>(ApiResponse<T>? data = null)
    {
        if (data == null)
        {
            return Ok();
        }
        return data.Status switch
        {
            (int)HttpStatusCode.OK => Ok(data),
            (int)HttpStatusCode.Created => Created("", data),
            (int)HttpStatusCode.BadRequest => BadRequest(data),
            (int)HttpStatusCode.UnprocessableEntity => UnprocessableEntity(data),
            (int)HttpStatusCode.Unauthorized => Unauthorized(data),
            (int)HttpStatusCode.Forbidden => Forbid(),
            (int)HttpStatusCode.NotFound => NotFound(data),
            _ => StatusCode(data.Status, data),
        };
    }

    [NonAction]
    public IActionResult BaseResult(ApiResponse? data = null)
    {
        if (data == null)
        {
            return Ok();
        }
        return data.Status switch
        {
            (int)HttpStatusCode.OK => Ok(data),
            (int)HttpStatusCode.Created => Created("", data),
            (int)HttpStatusCode.BadRequest => BadRequest(data),
            (int)HttpStatusCode.UnprocessableEntity => UnprocessableEntity(data),
            (int)HttpStatusCode.Unauthorized => Unauthorized(data),
            (int)HttpStatusCode.Forbidden => Forbid(),
            (int)HttpStatusCode.NotFound => NotFound(data),
            _ => StatusCode(data.Status, data),
        };
    }

    protected Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? User.FindFirst("userId")?.Value;

        return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
    }

    protected string GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value
            ?? User.FindFirst("role")?.Value
            ?? string.Empty;
    }
}
