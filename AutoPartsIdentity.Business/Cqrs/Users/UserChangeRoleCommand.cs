using System.Net;
using AutoPartsIdentity.Core.Results;
using AutoPartsIdentity.DataAccess.Enums;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserChangeRoleCommand : IRequest<IDataResult<object>>
{
    public ChangeUserRoleFormDto Form { get; }

    public UserChangeRoleCommand(ChangeUserRoleFormDto form)
    {
        Form = form;
    }

    public class Handler : IRequestHandler<UserChangeRoleCommand, IDataResult<object>>
    {
        #region DI

        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserChangeRoleCommand> _validator;

        public Handler(UserManager<User> userManager, IValidator<UserChangeRoleCommand> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(UserChangeRoleCommand request, CancellationToken ct)
        {
            #region Validation

            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return new ErrorDataResult<object>("Validation error", HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            #endregion

            var user = await _userManager.FindByIdAsync(request.Form.UserId.ToString());
            if (user == null)
                return new ErrorDataResult<object>("User not found", HttpStatusCode.NotFound);

            if (IsSystemAdmin(user))
                return new ErrorDataResult<object>("Changing the system administrator role is not allowed", HttpStatusCode.Forbidden);

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return new ErrorDataResult<object>(
                        string.Join("; ", removeResult.Errors.Select(e => e.Description)),
                        HttpStatusCode.BadRequest);
            }

            var addResult = await _userManager.AddToRoleAsync(user, request.Form.Role.Trim());
            if (!addResult.Succeeded)
                return new ErrorDataResult<object>(
                    string.Join("; ", addResult.Errors.Select(e => e.Description)),
                    HttpStatusCode.BadRequest);

            return new SuccessDataResult<object>("Role changed successfully");
        }

        private static bool IsSystemAdmin(User user)
        {
            return string.Equals(user.UserName, SystemAdminConstants.UserName, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(user.Email, SystemAdminConstants.Email, StringComparison.OrdinalIgnoreCase);
        }
    }
}
