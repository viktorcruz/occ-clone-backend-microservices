using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries
{
    public class GetAllUsersQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {
        #region Constructor
        public GetAllUsersQuery()
        {
            
        }
        #endregion
    }
}
