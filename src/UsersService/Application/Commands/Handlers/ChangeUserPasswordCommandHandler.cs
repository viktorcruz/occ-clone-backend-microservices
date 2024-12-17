using MediatR;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Audit;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using SharedKernel.Security.Extensions;
using System.Security.Claims;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public ChangeUserPasswordCommandHandler(
            IUserDomain userDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventPublisherService eventPublisherService,
            IHttpContextAccessor contextAccessor
            )
        {
            _userDomain = userDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _eventPublisherService = eventPublisherService;
            _endpointResponse = endpointResponse;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userDomain.GetByEmailAsync(request.Email);
                if(existingUser.Details == null || !existingUser.ResultStatus)
                {
                    throw new Exception("Invalid Credentials");
                }
                if(!Convert.ToBoolean(existingUser.Details?.IsActive) && !Convert.ToBoolean(existingUser.Details?.IsRegistrationConfirmed))
                {
                    throw new Exception("User not yet activated");
                }

                bool passHashMatch = PasswordHashingExtensions.VerifyPassword(request.CurrentPassword, existingUser.Details.PasswordHash);

                if(!passHashMatch)
                {
                    throw new Exception("Invalid Credentials");
                }

                string hashedPassword = PasswordHashingExtensions.HashPassword(request.NewPassword);
                bool isMatch = PasswordHashingExtensions.VerifyPassword(request.NewPassword, hashedPassword);

                if(!isMatch)
                {
                    throw new Exception("");
                }

                var response = await _userDomain.ChangeUserPasswordAsync(existingUser.Details.IdUser, hashedPassword, existingUser.Details.Email);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successfully updated";

                    var newPasswd = AuditDataExtension.CreatePasswordAuditData(existingUser.Details.IdUser, existingUser.Details.Email, _contextAccessor.GtePerformedBy());

                    var auditEvent = AuditDataExtension.CreateAuditEvent(newPasswd, _contextAccessor.GtePerformedBy(), true, _endpointResponse.Message);

                    await _eventPublisherService.PublishEventAsync(
                        entityName: auditEvent.EntityName,
                        operationType: auditEvent.OperationType,
                        success: auditEvent.Success,
                        performedBy: auditEvent.PerformedBy,
                        reason: auditEvent.Reason,
                        additionalData: newPasswd,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey()
                    );                
                }
            }
            catch(Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Update);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error when cahnging password: {ex.Message}";

                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "unkown"
                };
                
                var auditEvent = AuditDataExtension.CreateAuditEvent(errorEvent, _contextAccessor.GtePerformedBy(), false, _endpointResponse.Message);
  
                await _eventPublisherService.PublishEventAsync(
                    entityName: auditEvent.EntityName,
                    operationType: auditEvent.OperationType,
                    success: auditEvent.Success,
                    performedBy: auditEvent.PerformedBy,
                    reason: auditEvent.Reason,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey()
                );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
