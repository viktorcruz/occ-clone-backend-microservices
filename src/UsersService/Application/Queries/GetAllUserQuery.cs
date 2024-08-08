using MediatR;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries
{
    public class GetAllUserQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {

        public class TaskQueryHandler : IRequestHandler<GetAllUserQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
        {
            #region Properties
            private readonly IUserDomain _usersDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            #endregion

            #region Constructor
            public TaskQueryHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler)
            {
                _usersDomain = usersDomain;
                _globalExceptionHandler = globalExceptionHandler;
            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
            {
                var endpointResponse = new EndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>();
                try
                {
                    var users = await _usersDomain.GetAllUsersAsync();
                    endpointResponse.Data = users;

                    if (endpointResponse.Data.ResultStatus)
                    {
                        endpointResponse.IsSuccess = true;
                        endpointResponse.Message = "Successful";
                    }
                    else
                    {
                        endpointResponse.IsSuccess = false;
                        endpointResponse.Message = "Users not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "GetAllUserQuery.Handle");
                    endpointResponse.IsSuccess = false;
                    endpointResponse.Message = ex.Message;
                }
                return endpointResponse;
            }
            #endregion
        }

    }
}
