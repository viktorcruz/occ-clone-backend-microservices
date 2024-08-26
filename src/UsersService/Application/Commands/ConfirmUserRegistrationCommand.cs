using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;

namespace UsersService.Application.Commands
{
    /// <summary>
    /// Representa la accion de confirmar el registro de un usuario, que puede ser un paso 
    /// especifico en el flujo de trabajo
    /// </summary>
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
