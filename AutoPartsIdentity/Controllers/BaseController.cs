using System.Net;
using Microsoft.AspNetCore.Mvc;
using IResult = AutoPartsIdentity.Core.Results.IResult;

namespace AutoPartsIdentity.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult Return(IResult result)
    {
        return result.StatusCode switch
        {
            HttpStatusCode.BadRequest => BadRequest(result),
            HttpStatusCode.NotFound => NotFound(result),
            HttpStatusCode.Unauthorized => Unauthorized(result),
            HttpStatusCode.InternalServerError => BadRequest(result),
            HttpStatusCode.MethodNotAllowed => BadRequest(result),
            HttpStatusCode.Forbidden => StatusCode(403, result),
            HttpStatusCode.Conflict => StatusCode(409, result),
            HttpStatusCode.NotModified => StatusCode(304, result),
            HttpStatusCode.NotAcceptable => StatusCode(406, result),
            _ => Ok(result)
        };
    }
}