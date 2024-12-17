using MediatR;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Audit;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public DeleteUserCommandHandler(
            IUserDomain usersDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
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
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _usersDomain.DeleteUserAsync(request.idUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "User deleted successful";


                    var additionalData = new
                    {
                        IdUser = request.idUser,
                        IsDeleted = true
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.User.ToEntityName(),
                        operationType: AuditOperationType.Delete.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Delete_Success.ToRoutingKey()
                    );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Delete);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error deleting user: {ex.Message}";

                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.User.ToEntityName(),
                    operationType: AuditOperationType.Delete.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Delete_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
