using MediatR;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using UsersService.Application.DTO;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public UpdateUserCommandHandler(
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

        public async Task<IEndpointResponse<IDatabaseResult>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = new UserRetrieveDTO
                {
                    IdUser = request.IdUser,
                    IdRole = request.IdRole,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    IsActive = request.IsActive,
                };

                var response = await _usersDomain.UpdateUserAsync(user);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "User updated sucessful";

                    var additionalData = new
                    {
                        IdUser = user.IdUser,
                        IdRole = user.IdRole,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        IsActive = user.IsActive,
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.User.ToEntityName(),
                        operationType: AuditOperationType.Update.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Update);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating user: {ex.Message}";
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.User.ToEntityName(),
                    operationType: AuditOperationType.Update.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
    }
}
