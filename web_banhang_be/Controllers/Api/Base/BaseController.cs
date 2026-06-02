using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebBanHang.Models.Common;

namespace WebBanHang.Controllers.Api.Base
{
    [ApiController]
    public class BaseController:ControllerBase
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
        public IActionResult BaseResult(ApiResponse data = null)
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
    }
}
