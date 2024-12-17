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
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public UpdateUserProfileCommandHandler(
            IUserDomain userDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
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
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userProfile = new UserProfileDTO
                {
                    IdUser = request.IdUser,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                };

                var response = await _userDomain.UpdateUserProfileAsync(userProfile);
                _endpointResponse.Result = response;

                if (response == null || !response.ResultStatus)
                {
                    throw new Exception($"User not found {request.IdUser}");
                }

                _endpointResponse.IsSuccess = true;
                _endpointResponse.Message = "Profile updated successful";

                var additionalData = new
                {
                    IdUser = userProfile.IdUser,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    Email = userProfile.Email
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
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Update);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating profile: {ex.Message}";

                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.User.ToEntityName(),
                    operationType: AuditOperationType.Update.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
