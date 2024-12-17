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
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public GetAllUsersQueryHandler(
            IUserDomain usersDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService,
            IHttpContextAccessor contextAccessor
            )
        {
            _usersDomain = usersDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _usersDomain.GetAllUsersAsync();
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        TotalUsers = response.Details.Count,
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.User.ToEntityName(),
                        operationType: AuditOperationType.GetAll.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.GetAll_Success.ToRoutingKey()
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
                    operationType: AuditOperationType.GetAll.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.GetAll_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
