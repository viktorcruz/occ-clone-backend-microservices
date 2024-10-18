using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;

namespace UsersService.Application.Commands
{
    public class ConfirmUserRegistrationCommand : IRequest<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
    {
        #region Properties
        public int IdUser { get; set; }
        #endregion

        #region Constructor
        public ConfirmUserRegistrationCommand(int userId)
        {
            this.IdUser = userId;
        }
        #endregion
    }
}
