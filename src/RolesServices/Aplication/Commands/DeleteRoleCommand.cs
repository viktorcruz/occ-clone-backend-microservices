using MediatR;
using RolesServices.Aplication.Dto;
using RolesServices.Domain.Interface;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Aplication.Commands
{
    public class DeleteRoleCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            public int IdRole { get; set; }
            #endregion

            #region Constructor
            public TaskCommand(int roleId)
            {
                IdRole = roleId;
            }
            #endregion
        }
        public class DeleteRoleCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            private readonly IRoleDomain _roleDomain;
            private readonly IGlobalExceptionHandler _glocalExceptionHandler;
            private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
            #endregion

            #region Constructor
            public DeleteRoleCommandHandler(IRoleDomain roleDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<IDatabaseResult> endpointResponse)
            {
                _roleDomain = roleDomain;
                _glocalExceptionHandler = globalExceptionHandler;
                _endpointResponse = endpointResponse;
            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<IDatabaseResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var response = await _roleDomain.DeleteRoleAsync(request.IdRole);
                    _endpointResponse.Result = response;

                    if (response != null && response.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "Role deleted successful";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = response?.ResultMessage ?? "Role not deleted";
                    }
                }
                catch (Exception ex)
                {
                    _glocalExceptionHandler.HandleGenericException<string>(ex, "DeleteRoleCommand.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = $"Error deleting role: {ex.Message}";
                }
                return _endpointResponse;
            }
            #endregion
        }
    }
}
