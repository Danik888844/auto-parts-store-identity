using AutoPartsIdentity.Business.Cqrs.Users;
using AutoParts.DataAccess.Models.DtoModels;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserFormDto form)
    {
        return Return(await _mediator.Send(new UserCreateCommand(form)));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserFormDto form)
    {
        if (id != form.Id)
            return BadRequest();
        return Return(await _mediator.Send(new UserUpdateCommand(form)));
    }
    
    [Authorize]
    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new UserGetListCommand(form)));
    }

    [Authorize]
    [HttpPut("role")]
    public async Task<IActionResult> ChangeRole(ChangeUserRoleFormDto form)
    {
        return Return(await _mediator.Send(new UserChangeRoleCommand(form)));
    }

    [AllowAnonymous]
    [HttpPost("display-names")]
    public async Task<IActionResult> GetDisplayNames([FromBody] UserDisplayNamesRequestDto request)
    {
        return Return(await _mediator.Send(new UserGetDisplayNamesCommand(request.UserIds ?? new List<string>())));
    }
}