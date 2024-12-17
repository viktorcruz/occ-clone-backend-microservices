using MediatR;
using SharedKernel.Interfaces.Response;

namespace UsersService.Application.Commands
{
    public class DeleteUserCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int idUser { get; set; }
        #endregion

        #region Constructor
        public DeleteUserCommand(int userId)
        {
            idUser = userId;
        }
        #endregion
    }
}
