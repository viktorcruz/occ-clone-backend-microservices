using MediatR;
using SharedKernel.Interface;

namespace UsersService.Application.Commands
{
    public class ActivateUserCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdUser { get; set; }
        #endregion

        #region Constructor
        public ActivateUserCommand(int userId)
        {
            IdUser = userId;
        }
        #endregion
    }
}
