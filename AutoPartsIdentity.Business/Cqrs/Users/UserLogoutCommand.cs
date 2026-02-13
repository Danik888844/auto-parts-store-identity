using System.Net;
using System.Security.Claims;
using AutoPartsIdentity.Business.Services.Interfaces;
using AutoPartsIdentity.Core.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserLogoutCommand : IRequest<IDataResult<object>>
{
    public class Handler : IRequestHandler<UserLogoutCommand, IDataResult<object>>
    {
        #region DI
        
        private readonly ITokenCacheService _tokenCacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(ITokenCacheService tokenCacheService, IHttpContextAccessor httpContextAccessor)
        {
            _tokenCacheService = tokenCacheService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(UserLogoutCommand request, CancellationToken ct)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrWhiteSpace(userId))
                return new ErrorDataResult<object>("User not found", HttpStatusCode.Unauthorized);

            var result = _tokenCacheService.UnregisterUser(userId);
            
            if (!result)
                return new ErrorDataResult<object>("Token not found", HttpStatusCode.NotFound);
            
            return new SuccessDataResult<object>("Successfully logged out");
        }
    }
}

