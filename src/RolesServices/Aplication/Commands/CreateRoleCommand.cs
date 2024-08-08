using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Aplication.Commands
{
    public class CreateRoleCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<DatabaseResult>>
        {

        }
    }
}
