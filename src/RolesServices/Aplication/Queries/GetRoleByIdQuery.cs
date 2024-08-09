using MediatR;
using RolesServices.Aplication.Dto;
using RolesServices.Domain.Interface;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Aplication.Queries
{
    public class GetRoleByIdQuery
    {
        public class TaskQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<RoleDTO>>>
        {
            #region Properties
            public int IdRole { get; set; }
            #endregion

            #region Constructor
            public TaskQuery(int roleId)
            {
                IdRole = roleId;
            }
            #endregion
        }

        public class GetRoleByIdQueryHandler : IRequestHandler<TaskQuery, IEndpointResponse<RetrieveDatabaseResult<RoleDTO>>>
        {
            #region Properties
            private readonly IRoleDomain _roleDomain;
            private readonly IEndpointResponse<RetrieveDatabaseResult<RoleDTO>> _endpointResponse;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            #endregion

            #region Constructor
            public GetRoleByIdQueryHandler(IRoleDomain roleDomain, IEndpointResponse<RetrieveDatabaseResult<RoleDTO>> endpointResponse, IGlobalExceptionHandler globalExceptionHandler)
            {
                _roleDomain = roleDomain;
                _endpointResponse = endpointResponse;
                _globalExceptionHandler = globalExceptionHandler;
            }
            #endregion

            #region Methos
            public async Task<IEndpointResponse<RetrieveDatabaseResult<RoleDTO>>> Handle(TaskQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var role = await _roleDomain.GetRoleByIdAsync(request.IdRole);

                    _endpointResponse.Result = role;

                    if (_endpointResponse.Result.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "Role found";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = "Role not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "GetRoleByIdQuery.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = $"Error: {ex.Message}";
                }
                return _endpointResponse;
            }
            #endregion
        }
    }
}
