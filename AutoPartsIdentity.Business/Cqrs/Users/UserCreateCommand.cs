
using System.Net;
using AutoPartsIdentity.Core.Results;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserCreateCommand : IRequest<IDataResult<object>>
{
    public CreateUserFormDto Form { get; }
    
    public UserCreateCommand(CreateUserFormDto form)
    {
        Form = form;
    }

    public class Handler : IRequestHandler<UserCreateCommand, IDataResult<object>>
    {
        #region DI
        
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserCreateCommand> _validator;

        public Handler(UserManager<User> userManager, IValidator<UserCreateCommand> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(UserCreateCommand request, CancellationToken ct)
        {
            #region Validation
            
            var validationOfForm = await _validator.ValidateAsync(request, ct);
            if(!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Validation error", HttpStatusCode.BadRequest, 
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());
            
            #endregion

            var user = new User
            {
                UserName = request.Form.UserName.Trim(),
                Email = request.Form.Email.Trim(),
                PhoneNumber = request.Form.PhoneNumber?.Trim() ?? String.Empty,
                FirstName = request.Form.FirstName.Trim(),
                LastName = request.Form.LastName.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Form.Password);
            if (!result.Succeeded)
                return new ErrorDataResult<object>(string.Join("; ", result.Errors.Select(e => e.Description)), HttpStatusCode.BadRequest);
            
            return new SuccessDataResult<object>("Success");
        }
    }
}