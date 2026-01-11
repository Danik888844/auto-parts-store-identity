using AutoPartsIdentity.Business.Cqrs.Users;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace AutoPartsIdentity.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    #region DI

    private readonly IMediator _mediator;
    
    #endregion

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(CreateUserFormDto form)
    {
        return Return(await _mediator.Send(new UserCreateCommand(form)));
    }
}