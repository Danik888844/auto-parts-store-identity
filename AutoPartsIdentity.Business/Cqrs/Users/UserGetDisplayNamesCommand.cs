using AutoPartsIdentity.Core.Results;
using AutoPartsIdentity.DataAccess.Dals;
using MediatR;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserGetDisplayNamesCommand : IRequest<IDataResult<Dictionary<string, string>>>
{
    public List<string> UserIds { get; }

    public UserGetDisplayNamesCommand(List<string> userIds)
    {
        UserIds = userIds ?? new List<string>();
    }

    public class Handler : IRequestHandler<UserGetDisplayNamesCommand, IDataResult<Dictionary<string, string>>>
    {
        private readonly IUserDal _userDal;

        public Handler(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public async Task<IDataResult<Dictionary<string, string>>> Handle(UserGetDisplayNamesCommand request, CancellationToken ct)
        {
            var displayNames = await _userDal.GetDisplayNamesByIdsAsync(request.UserIds, ct);
            return new SuccessDataResult<Dictionary<string, string>>(displayNames);
        }
    }
}
