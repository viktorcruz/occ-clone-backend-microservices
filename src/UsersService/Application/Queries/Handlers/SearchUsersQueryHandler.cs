using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Queries.Handlers
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public SearchUsersQueryHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> endpointResponse,
            EventBusRabbitMQ eventBus
            )
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FirstName) && string.IsNullOrEmpty(request.LastName) && string.IsNullOrEmpty(request.Email))
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "At least one search criteria must be provided (FirstName, LastName, Email)";
                    return _endpointResponse;
                }

                var response = await _userDomain.SearchUsersAsync(request.FirstName, request.LastName, request.Email);

                if (response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Users retrieved successfully";
                    _endpointResponse.Result = response;

                    var additionalData = new
                    {
                        IdUser = request.FirstName,
                        FirstName = request.FirstName,
                        NameUser = request.LastName,
                        Email = request.Email,
                    };

                    var entityOperationEvent = new EntityOperationEvent(
                        entityName: "User",
                        operationType: "Search",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("UserExchange", "UserSearched", entityOperationEvent);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "No users found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "SearchUsersQueryHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "Search", success: false, performedBy: "AdminUser");

                _eventBus.Publish("UserExchange", "SearchUsersFailed", failedEvent);
            }

            return _endpointResponse;
        }
        #endregion
    }
}
