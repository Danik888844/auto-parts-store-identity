
using System.Net;
using AutoMapper;
using AutoPartsIdentity.Business.Services.Interfaces;
using AutoPartsIdentity.Core.Results;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserLoginCommand : IRequest<IDataResult<object>>
{
    public LoginFormDto Form { get; }
    
    public UserLoginCommand(LoginFormDto form)
    {
        Form = form;
    }

    public class Handler : IRequestHandler<UserLoginCommand, IDataResult<object>>
    {
        #region DI
        
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly IMapper _mapper;

        public Handler(UserManager<User> userManager, SignInManager<User> signInManager, 
            ITokenCacheService tokenCacheService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenCacheService = tokenCacheService;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(UserLoginCommand request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Form.Login) || string.IsNullOrWhiteSpace(request.Form.Password))
                return new ErrorDataResult<object>("Invalid login or password", HttpStatusCode.BadRequest);

            // find by username / email
            var user = await _userManager.FindByNameAsync(request.Form.Login)
                       ?? await _userManager.FindByEmailAsync(request.Form.Login);

            if (user is null)
                return new ErrorDataResult<object>("Invalid login or password", HttpStatusCode.BadRequest);

            if (!user.IsActive) 
                return new ErrorDataResult<object>("User inactive", HttpStatusCode.MethodNotAllowed);

            if (await _userManager.IsLockedOutAsync(user))
                return new ErrorDataResult<object>("User locked", HttpStatusCode.MethodNotAllowed);
            
            var result = await _signInManager.CheckPasswordSignInAsync(
                user, request.Form.Password, lockoutOnFailure: false);
            
            if (result.IsNotAllowed)
                return new ErrorDataResult<object>("User not allowed", HttpStatusCode.MethodNotAllowed);

            if (!result.Succeeded)
                return new ErrorDataResult<object>("Invalid login or passwords", HttpStatusCode.BadRequest);
            
            var userDto = _mapper.Map<UserDto>(user);

            var userToken = _tokenCacheService.RegisterUser(userDto);
            if(userToken == null)
                return new ErrorDataResult<object>("Token not created", HttpStatusCode.BadRequest);
            
            return new SuccessDataResult<object>(userToken, "Success");
        }
    }
}