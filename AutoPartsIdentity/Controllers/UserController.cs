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
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginFormDto form)
    {
        return Return(await _mediator.Send(new UserLoginCommand(form)));
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return Return(await _mediator.Send(new UserLogoutCommand()));
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserFormDto form)
    {
        return Return(await _mediator.Send(new UserCreateCommand(form)));
    }
    
    [Authorize]
    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        return Return(await _mediator.Send(new UserGetListCommand()));
    }
}