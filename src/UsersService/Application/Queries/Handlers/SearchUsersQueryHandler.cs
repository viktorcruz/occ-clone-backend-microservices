using MediatR;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries.Handlers
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, IEndpointResponse<RetrieveDatabaseResult<List<SearchUsersDTO>>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<SearchUsersDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public SearchUsersQueryHandler(
            IUserDomain userDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<SearchUsersDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _userDomain = userDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<SearchUsersDTO>>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
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

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "User",
                        operationType: "SEARCH",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "User not found",
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "No users found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "User",
                        operationType: "SEARCH",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Data not found",
                        additionalData: response,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.FetchAll);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: "User",
                    operationType: "SEARCH",
                    success: true,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Search_Error.ToRoutingKey()
                    );
            }

            return _endpointResponse;
        }
        #endregion
    }
}
