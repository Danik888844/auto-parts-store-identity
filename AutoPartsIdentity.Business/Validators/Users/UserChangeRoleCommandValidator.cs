using AutoPartsIdentity.Business.Cqrs.Users;
using AutoPartsIdentity.DataAccess.Enums;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace AutoPartsIdentity.Business.Validators.Users;

public class UserChangeRoleCommandValidator : AbstractValidator<UserChangeRoleCommand>
{
    private static readonly string[] AllowedRoles = { UserRoleEnum.Administrator, UserRoleEnum.Seller };

    public UserChangeRoleCommandValidator(UserManager<User> userManager)
    {
        RuleFor(x => x.Form.UserId)
            .NotEmpty()
            .WithMessage("UserId is required")
            .MustAsync(async (userId, ct) =>
            {
                var user = await userManager.FindByIdAsync(userId.ToString());
                return user != null;
            })
            .WithMessage("User not found");

        RuleFor(x => x.Form.Role)
            .NotEmpty()
            .WithMessage("Role is required")
            .Must(role => AllowedRoles.Contains(role?.Trim() ?? string.Empty))
            .WithMessage($"Role must be one of: {string.Join(", ", AllowedRoles)}");
    }
}
