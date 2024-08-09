using AutoMapper;
using MediatR;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries
{
    public class GetUserByIdQuery
    {
        public class TaskQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
        {
            #region Properties
            public int Id { get; set; }
            #endregion

            #region Constructor
            public TaskQuery(int id)
            {
                this.Id = id;
            }
            #endregion
        }

        public class TaskQueryHandler : IRequestHandler<TaskQuery, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
        {
            #region Properties
            private readonly IUserDomain _userDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            private readonly IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> _endpointResponse;
            #endregion

            #region Constructor
            public TaskQueryHandler(IUserDomain userDomain, IMapper mapper, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> endpointResponse)
            {
                _userDomain = userDomain;
                _globalExceptionHandler = globalExceptionHandler;
                _endpointResponse = endpointResponse;
            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>> Handle(TaskQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var response = await _userDomain.GetUserByIdAsync(request.Id);
                    _endpointResponse.Result = response;

                    if (response != null && response.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "Successful";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "GetUserByIdQuery.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = ex.Message;
                }
                return _endpointResponse;
            }
            #endregion
        }
    }
}
