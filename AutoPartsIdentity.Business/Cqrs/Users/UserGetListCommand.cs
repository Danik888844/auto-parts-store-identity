using AutoMapper;
using AutoPartsIdentity.Core.Results;
using AutoPartsIdentity.DataAccess.Dals;
using MediatR;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserGetListCommand : IRequest<IDataResult<object>>
{
    public class Handler : IRequestHandler<UserGetListCommand, IDataResult<object>>
    {
        #region DI
        
        private readonly IMapper _mapper;
        private readonly IUserDal _userDal;

        public Handler(IMapper mapper, IUserDal userDal)
        {
            _mapper = mapper;
            _userDal = userDal;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(UserGetListCommand request, CancellationToken ct)
        {

            var users = await _userDal.GetUsersWithRolesAsync(1, 9999, ct);
            // map<UserDto>(users)
            
            return new SuccessDataResult<object>(users, "Success");
        }
    }
}