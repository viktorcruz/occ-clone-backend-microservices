using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;

namespace UsersService.Application.Queries
{
    public class GetAllUsersQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {
        #region Constructor
        public GetAllUsersQuery() { }
        #endregion
    }
}
