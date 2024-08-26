using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;

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
