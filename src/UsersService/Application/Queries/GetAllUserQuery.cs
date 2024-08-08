using AutoMapper;
using MediatR;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

namespace UsersService.Application.Queries
{
    public class GetAllUserQuery : IRequest<IApiResponse<SpRetrieveResult<List<UserRetrieveDTO>>>>
    {

        public class TaskQueryHandler : IRequestHandler<GetAllUserQuery, IApiResponse<SpRetrieveResult<List<UserRetrieveDTO>>>>
        {
            #region Properties
            private readonly IUsersDomain _usersDomain;
            private readonly IExceptionManagement _exceptionManagement;
            #endregion

            #region Constructor
            public TaskQueryHandler(IUsersDomain usersDomain, IExceptionManagement exceptionManagement)
            {
                _usersDomain = usersDomain;
                _exceptionManagement = exceptionManagement;
            }
            #endregion

            #region Methods
            public async Task<IApiResponse<SpRetrieveResult<List<UserRetrieveDTO>>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
            {
                var apiResponse = new ApiResponse<SpRetrieveResult<List<UserRetrieveDTO>>>();
                try
                {
                    var users = await _usersDomain.GetAllUsersAsync();
                    apiResponse.Data = users;

                    if (apiResponse.Data.ResultStatus)
                    {
                        apiResponse.IsSuccess = true;
                        apiResponse.Message = "Successful";
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.Message = "Users not found";
                    }
                }
                catch (Exception ex)
                {
                    _exceptionManagement.HandleGenericException<string>(ex, "GetAllUserQuery.Handle");
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ex.Message;
                }
                return apiResponse;
            }
            #endregion
        }

    }
}
