using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Audit;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Service;

namespace AuthService.Infrastructure.Adapters
{
    public class ConfirmRegisterAdapter : IConfirmRegisterPort
    {
        #region Properties
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICorrelationService _correlationService;
        #endregion

        #region Constructor
        public ConfirmRegisterAdapter(
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler,
            IHttpContextAccessor contextAccessor,
            ICorrelationService correlationService
            )
        {
            _userRepository = userRepository;
            _applicationExceptionHandler = applicationExceptionHandler;
            _eventPublisherService = eventPublisherService;
            _contextAccessor = contextAccessor;
            _correlationService = correlationService;
        }
        #endregion

        #region Methods
        public async Task<ConfirmRegisterResponseDTO?> ConfirmRegisterAsync(ConfirmRegisterCommand command)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(command.Email);

                if (existingUser.Details == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                var updateUser = await _userRepository.ChangeUserStatusAsync(existingUser.Details.IdUser, existingUser.Details.Email);

                if (updateUser == null || !updateUser.ResultStatus)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                var response = new ConfirmRegisterResponseDTO
                {
                    IdUser = existingUser.Details.IdUser,
                    Email = existingUser.Details.Email,
                    IsActive = updateUser.ResultStatus ? true : false,
                    ActiveStatusDescription = "User has been activated",
                    IsRegistrationConfirmed = updateUser.ResultStatus ? true : false,
                    RegistrationConfirmationDescription = "User registration is confirmed",
                };


                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Authorize.ToEntityName(),
                    operationType: AuditOperationType.Login.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: "User's email has been confirmed",
                    additionalData: response,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Confirmation_Success.ToRoutingKey()
                    );

                return response;
            }
            catch (Exception ex)
            {
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? string.Empty
                };

                //var auditEvent = new AuditEventEntity
                //{
                //    EntityName = AuditEntityType.Authorize.ToEntityName(),
                //    OperationType = AuditOperationType.Login.ToOperationType(),
                //    Success = true,
                //    PerformedBy = "",
                //    Reason = ex.Message,
                //    AdditionalData = errorEvent
                //};

                //var capturaDTO = new AuditExceptionDTO
                //{
                //    Exception = ex,
                //    ApplicationLayer = ApplicationLayer.Adapter,
                //    ActionType = ActionType.Update,
                //    AuditTracking = _correlationService.GetCorrelationId()
                //};

                var capturaDTO = AuditDataExtension.CreateAuditException(ex, ApplicationLayer.Adapter, ActionType.Update, _correlationService.GetCorrelationId());

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Authorize.ToEntityName(),
                    operationType: AuditOperationType.Login.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Confirmation_Error.ToRoutingKey()
                    );

                _applicationExceptionHandler.CaptureException<string>(capturaDTO);

                return null;
            }
        }
        #endregion
    }
}
