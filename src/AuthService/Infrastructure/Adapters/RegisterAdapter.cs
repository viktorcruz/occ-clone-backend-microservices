using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using AuthService.Infrastructure.Security;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Events.User;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;

namespace AuthService.Infrastructure.Adapters
{
    public class RegisterAdapter : IRegisterPort
    {
        #region Properties
        private readonly IRegisterUserPort _registerRepository;
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public RegisterAdapter(
            IRegisterUserPort registerRepository,
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler,
            IHttpContextAccessor contextAccessor
            )
        {
            _registerRepository = registerRepository;
            _userRepository = userRepository;
            _eventPublisherService = eventPublisherService;
            _applicationExceptionHandler = applicationExceptionHandler;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<RegisterUserDTO?> RegisterAsync(RegisterCommand command)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(command.Email);

                if (existingUser.Details != null || existingUser.ResultStatus)
                {
                    throw new Exception("Email alredy exists, please select anther email");
                }

                string hashedPassword = PasswordHasher.HashPassword(command.Password);

                bool isMatch = PasswordHasher.VerifyPassword(command.Password, hashedPassword);

                if (!isMatch)
                {
                    throw new Exception("Password incorrect.");
                }

                var createdEvent = new UserCreatedEvent
                {
                    IdRole = command.IdRole,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                    PasswordHash = hashedPassword
                };

                var response = await _registerRepository.AddAsync(createdEvent);
                createdEvent.IdUser = response.AffectedRecordId;

                if (!response.ResultStatus && response.AffectedRecordId == 0)
                {
                    throw new Exception(response.ExceptionMessage);
                }

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Authorize.ToEntityName(),
                    operationType: AuditOperationType.Register.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: "User has registered",
                    additionalData: createdEvent,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Register_Success.ToRoutingKey()
                    );

                var userResponse = new RegisterUserDTO
                {
                    IdRole = createdEvent.IdRole,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = createdEvent.Email,
                    Password = "####"
                };

                return userResponse;
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Adapter, ActionType.Register);
                var registerErrorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
                var failedEvent = new UserCreationFailedEvent
                {
                    IdRole = command.IdRole,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                    Reason = registerErrorEvent
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Authorize.ToEntityName(),
                    operationType: AuditOperationType.Register.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: failedEvent,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Register_Error.ToRoutingKey()
                    );

                return new RegisterUserDTO { IdRole = command.IdRole, Email = command.Email, Password = command.Password };
            }
        }
        #endregion
    }
}
