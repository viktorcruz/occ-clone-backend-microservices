using AutoMapper;
using MediatR;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

namespace UsersService.Application.Queries
{
    public class GetUserByIdQuery
    {
        public class TaskQuery : IRequest<IApiResponse<SpRetrieveResult<UserRetrieveDTO>>>
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

        public class TaskQueryHandler : IRequestHandler<TaskQuery, IApiResponse<SpRetrieveResult<UserRetrieveDTO>>>
        {
            #region Properties
            private readonly IUsersDomain _userDomain;
            private readonly IMapper _mapper;
            private readonly IExceptionManagement _exceptionManagement;
            #endregion

            #region Constructor
            public TaskQueryHandler(IUsersDomain userDomain, IMapper mapper, IExceptionManagement exceptionManagement)
            {
                _userDomain = userDomain;
                _mapper = mapper;
                _exceptionManagement = exceptionManagement;
            }
            #endregion

            #region Methods
            public async Task<IApiResponse<SpRetrieveResult<UserRetrieveDTO>>> Handle(TaskQuery request, CancellationToken cancellationToken)
            {
                var apiResponse = new ApiResponse<SpRetrieveResult<UserRetrieveDTO>>();
                try
                {
                    var user = await _userDomain.GetUserByIdAsync(request.Id);
                    //apiResponse.Data = _mapper.Map<SpRetrieveResult<UserDTO>>(user);
                    apiResponse.Data = user;

                    if (apiResponse.Data.ResultStatus)
                    {
                        apiResponse.IsSuccess = true;
                        apiResponse.Message = "Successful";
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.Message = "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _exceptionManagement.HandleGenericException<string>(ex, "GetUserByIdQuery.Handle");
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ex.Message;
                }
                return apiResponse;
            }
            #endregion
        }
    }
}
