using System.Net;
using AutoPartsIdentity.Core.Results;
using AutoPartsIdentity.DataAccess.Enums;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserUpdateCommand : IRequest<IDataResult<object>>
{
    public UpdateUserFormDto Form { get; }

    public UserUpdateCommand(UpdateUserFormDto form)
    {
        Form = form;
    }

    public class Handler : IRequestHandler<UserUpdateCommand, IDataResult<object>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserUpdateCommand> _validator;

        public Handler(UserManager<User> userManager, IValidator<UserUpdateCommand> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<IDataResult<object>> Handle(UserUpdateCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return new ErrorDataResult<object>("Validation error", HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            var user = await _userManager.FindByIdAsync(request.Form.Id.ToString());
            if (user == null)
                return new ErrorDataResult<object>("User not found", HttpStatusCode.NotFound);

            if (IsSystemAdmin(user))
                return new ErrorDataResult<object>("Editing the system administrator is not allowed", HttpStatusCode.Forbidden);

            user.FirstName = request.Form.FirstName.Trim();
            user.LastName = request.Form.LastName.Trim();
            user.Email = request.Form.Email.Trim();
            user.NormalizedEmail = _userManager.NormalizeEmail(request.Form.Email);
            user.PhoneNumber = request.Form.PhoneNumber?.Trim() ?? string.Empty;
            user.IsActive = request.Form.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ErrorDataResult<object>(
                    string.Join("; ", result.Errors.Select(e => e.Description)),
                    HttpStatusCode.BadRequest);

            return new SuccessDataResult<object>("User updated successfully");
        }

        private static bool IsSystemAdmin(User user)
        {
            return string.Equals(user.UserName, SystemAdminConstants.UserName, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(user.Email, SystemAdminConstants.Email, StringComparison.OrdinalIgnoreCase);
        }
    }
}
