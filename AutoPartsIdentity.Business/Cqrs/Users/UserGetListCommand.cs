using AutoParts.DataAccess.Models.DtoModels;
using AutoPartsIdentity.Core.Results;
using AutoPartsIdentity.DataAccess.Dals;
using MediatR;

namespace AutoPartsIdentity.Business.Cqrs.Users;

public class UserGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public UserGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class Handler : IRequestHandler<UserGetListCommand, IDataResult<object>>
    {
        private readonly IUserDal _userDal;

        public Handler(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public async Task<IDataResult<object>> Handle(UserGetListCommand request, CancellationToken ct)
        {
            var pageNumber = Math.Max(1, request.Form.PageNumber);
            var pageSize = Math.Clamp(request.Form.ViewSize, 1, 200);

            var (items, totalCount) = await _userDal.GetUserListAsync(pageNumber, pageSize, ct);

            var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

            var result = new
            {
                Items = items,
                Pagination = new PaginationReturnModel
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                }
            };

            return new SuccessDataResult<object>(result, "Success");
        }
    }
}