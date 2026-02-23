using AutoPartsIdentity.Business.Cqrs.Users;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsIdentity.Business.Validators.Users;

public class UserUpdateCommandValidator : AbstractValidator<UserUpdateCommand>
{
    public UserUpdateCommandValidator(UserManager<User> userManager)
    {
        RuleFor(x => x.Form.Id)
            .NotEmpty()
            .WithMessage("User id is required");

        RuleFor(x => x.Form.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("First name must be no more than 100 characters");

        RuleFor(x => x.Form.LastName)
            .MaximumLength(100)
            .WithMessage("Last name must be no more than 100 characters");

        RuleFor(x => x.Form.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .WithMessage("Valid email required, max 256 characters")
            .MustAsync(async (cmd, email, ct) =>
            {
                var user = await userManager.FindByIdAsync(cmd.Form.Id.ToString());
                if (user == null) return false;
                var normalized = userManager.NormalizeEmail(email);
                var existing = await userManager.Users
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized && u.Id != cmd.Form.Id, ct);
                return existing == null;
            })
            .WithMessage("Email already used by another user");

        RuleFor(x => x.Form)
            .MustAsync(async (form, ct) =>
            {
                var user = await userManager.FindByIdAsync(form.Id.ToString());
                return user != null;
            })
            .WithMessage("User not found");

        When(x => x.Form.PhoneNumber != null, () =>
        {
            RuleFor(x => x.Form.PhoneNumber)
                .MaximumLength(50)
                .WithMessage("Phone number must be no more than 50 characters");
        });
    }
}
