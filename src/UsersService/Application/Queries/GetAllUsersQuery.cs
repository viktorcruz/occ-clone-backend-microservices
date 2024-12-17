using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Response;
using UsersService.Application.DTO;

namespace UsersService.Application.Queries
{
    public class GetAllUsersQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {
        #region Constructor
        public GetAllUsersQuery() { }
        #endregion
    }
}
