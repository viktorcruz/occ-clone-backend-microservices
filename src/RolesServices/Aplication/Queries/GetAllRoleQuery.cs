using MediatR;
using RolesServices.Aplication.Dto;
using RolesServices.Domain.Interface;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Aplication.Queries
{
    public class GetAllRoleQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<RoleDTO>>>>
    {
        public class TaskQueryHandler : IRequestHandler<GetAllRoleQuery, IEndpointResponse<RetrieveDatabaseResult<List<RoleDTO>>>>
        {
            #region Properties
            private readonly IRoleDomain _roleDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            private readonly IEndpointResponse<RetrieveDatabaseResult<List<RoleDTO>>> _endpointResponse;
            #endregion

            #region Constructor
            public TaskQueryHandler(IRoleDomain roleDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<RetrieveDatabaseResult<List<RoleDTO>>> endpointResponse)
            {
                _roleDomain = roleDomain;
                _globalExceptionHandler = globalExceptionHandler;
                _endpointResponse = endpointResponse;
            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<RetrieveDatabaseResult<List<RoleDTO>>>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var response = await _roleDomain.GetAllRolesAsync();
                    _endpointResponse.Result = response;

                    if (response != null && response.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = response?.ResultMessage ?? "Roles not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "GetAllRoleQuery.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = $"Error: {ex.Message}";
                }
                return _endpointResponse;
            }
            #endregion
        }
    }
}
