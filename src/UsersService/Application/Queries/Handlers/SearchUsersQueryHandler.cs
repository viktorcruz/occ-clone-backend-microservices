using MediatR;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Responses;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using UsersService.Application.DTO;
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
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public SearchUsersQueryHandler(
            IUserDomain userDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<SearchUsersDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService,
            IHttpContextAccessor contextAccessor
            )
        {
            _userDomain = userDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
            _contextAccessor = contextAccessor;
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
                        entityName: AuditEntityType.User.ToEntityName(),
                        operationType: AuditOperationType.Search.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response?.ResultMessage ?? "User not found",
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Success.ToRoutingKey()
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
                    entityName: AuditEntityType.User.ToEntityName(),
                    operationType: AuditOperationType.Search.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
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
