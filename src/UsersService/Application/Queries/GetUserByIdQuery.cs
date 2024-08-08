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
            private readonly IMapper _mapper;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            #endregion

            #region Constructor
            public TaskQueryHandler(IUserDomain userDomain, IMapper mapper, IGlobalExceptionHandler globalExceptionHandler)
            {
                _userDomain = userDomain;
                _mapper = mapper;
                _globalExceptionHandler = globalExceptionHandler;
            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>> Handle(TaskQuery request, CancellationToken cancellationToken)
            {
                var endpointResponse = new EndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>();
                try
                {
                    var user = await _userDomain.GetUserByIdAsync(request.Id);
                    //endpointResponse.Data = _mapper.Map<GlobalExceptionHandler<UserDTO>>(user);
                    endpointResponse.Data = user;

                    if (endpointResponse.Data.ResultStatus)
                    {
                        endpointResponse.IsSuccess = true;
                        endpointResponse.Message = "Successful";
                    }
                    else
                    {
                        endpointResponse.IsSuccess = false;
                        endpointResponse.Message = "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "GetUserByIdQuery.Handle");
                    endpointResponse.IsSuccess = false;
                    endpointResponse.Message = ex.Message;
                }
                return endpointResponse;
            }
            #endregion
        }
    }
}
