using MediatR;
using RolesServices.Aplication.Dto;
using RolesServices.Domain.Interface;
using SharedKernel.Interface;

namespace RolesServices.Aplication.Commands
{
    public class UpdateRoleCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            public int IdRole { get; set; }
            public string RoleName { get; set; }
            #endregion

            #region Constructor
            public TaskCommand(RoleDTO roleDTO)
            {
                IdRole = roleDTO.IdRole;
                RoleName = roleDTO.RoleName;
            }
            #endregion
        }

        public class UpdateRoleCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            private readonly IRoleDomain _roleDomain;
            private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            #endregion

            #region Constructor
            public UpdateRoleCommandHandler(IRoleDomain roleDomain, IEndpointResponse<IDatabaseResult> endpointResponse, IGlobalExceptionHandler globalExceptionHandler)
            {
                _roleDomain = roleDomain;
                _endpointResponse = endpointResponse;
                _globalExceptionHandler = globalExceptionHandler;
            }
            #endregion


            #region Methods
            public async Task<IEndpointResponse<IDatabaseResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var role = new RoleDTO
                    {
                        IdRole = request.IdRole,
                        RoleName = request.RoleName,
                    };

                    var response = await _roleDomain.UpdateRoleAsync(role);
                    _endpointResponse.Result = response;

                    if (response != null && response.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "Role updated successful";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = response?.ResultMessage ?? "Role not updated";
                    }

                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "UpdateRoleCommand.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = $"Errro updating role: {ex.Message}";
                }
                return _endpointResponse;
            }
            #endregion
        }
    }
}
