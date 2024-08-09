using MediatR;
using RolesServices.Aplication.Dto;
using RolesServices.Domain.Interface;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Aplication.Commands
{
    public class CreateRoleCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            public string RoleName { get; set; }
            #endregion

            #region Constructor
            public TaskCommand(AddRoleDTO addRoleDTO)
            {
                RoleName = addRoleDTO.RoleName;
            }
            #endregion
        }

        public class CreateRoleCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            private readonly IRoleDomain _roleDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            #endregion

            #region Constructor
            public CreateRoleCommandHandler(IRoleDomain roleDomain, IGlobalExceptionHandler globalExceptionHandler)
            {
                _roleDomain = roleDomain;
                _globalExceptionHandler = globalExceptionHandler;
            }
            #endregion

            public async Task<IEndpointResponse<IDatabaseResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                var endpointResponse = new EndpointResponse<IDatabaseResult>();

                try
                {
                    var role = new RoleDTO
                    {
                        RoleName = request.RoleName
                    };

                    var response = await _roleDomain.CreateRoleAsync(role);
                    endpointResponse.Result = response;

                    if(response != null && response.ResultStatus)
                    {
                        endpointResponse.IsSuccess = true;
                        endpointResponse.Message = "Role created successful";
                    }
                    else
                    {
                        endpointResponse.IsSuccess = false;
                        endpointResponse.Message = response?.ResultMessage ?? "Role not created";
                    }
                }
                catch(Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "CreateRoleCommand.Handle");
                    endpointResponse.IsSuccess = false;
                    endpointResponse.Message = $"Error creating role: {ex.Message}";
                }
                return endpointResponse;
            }
        }
    }
}
