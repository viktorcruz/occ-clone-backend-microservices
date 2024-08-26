using MediatR;
using RabbitMQ.Client;
using SharedKernel.Interface;

namespace UsersService.Application.Commands
{
    /// <summary>
    /// Se usa para desactivar o eliminar un usuario, lo que es mas especifico que un simple delete
    /// </summary>
    public class DeactivateUserCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdUser { get; set; }
        #endregion

        #region Constructor
        public DeactivateUserCommand(int userId)
        {
            IdUser = userId;
        }
        #endregion
    }
}
