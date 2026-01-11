using AutoPartsIdentity.Business.Cqrs.Users;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsIdentity.Business.Validators.Users;

public class UserCreateCommandValidator : AbstractValidator<UserCreateCommand>
{
    public UserCreateCommandValidator(UserManager<User> userManager)
    {
        RuleFor(x => x.Form.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("No more 100 characters long");
        
        RuleFor(x => x.Form.UserName)
            .NotEmpty().MinimumLength(3).MaximumLength(50)
            .WithMessage("From 3 to 100 characters ranged")
            .MustAsync(async (cmd, userName, ct) =>
            {
                var norm = userManager.NormalizeName(userName);
                return !await userManager.Users.AnyAsync(u => u.NormalizedUserName == norm, ct);
            })
            .WithMessage("UserName already exists");

        RuleFor(x => x.Form.Email)
            .NotEmpty().EmailAddress().MaximumLength(256)
            .WithMessage("No more 256 characters long")
            .MustAsync(async (cmd, email, ct) =>
            {
                var norm = userManager.NormalizeEmail(email);
                return !await userManager.Users.AnyAsync(u => u.NormalizedEmail == norm, ct);
            })
            .WithMessage("Email already exists");
        
        RuleFor(x => x.Form.Password)
            .NotEmpty().MinimumLength(6).MaximumLength(100)
            .WithMessage("From 6 to 100 characters ranged");
    }
}