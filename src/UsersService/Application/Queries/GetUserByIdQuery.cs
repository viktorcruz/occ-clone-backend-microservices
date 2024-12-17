using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Response;
using UsersService.Application.DTO;

namespace UsersService.Application.Queries
{
    public class GetUserByIdQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
    {
        #region Properties
        public int IdUser { get; set; }
        #endregion

        #region Constructor
        public GetUserByIdQuery(int userId)
        {
            IdUser = userId;
        }
        #endregion
    }
}
