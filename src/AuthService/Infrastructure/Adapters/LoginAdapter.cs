using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using AuthService.Domain.Ports.Output.Services;
using AuthService.Infrastructure.Extensions;
using AuthService.Infrastructure.Security;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Service;

namespace AuthService.Infrastructure.Adapters
{
    public class LoginAdapter : ILoginPort
    {
        #region Properties
        private readonly IUserPort _userRepository;
        private readonly IRolePort _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICorrelationService _correlationService;
        #endregion

        #region Constructor
        public LoginAdapter(
            IUserPort userRepository,
            IRolePort roleRepository,
            ITokenService tokenService,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler,
            IHttpContextAccessor contextAccessor,
            ICorrelationService correlationService
            )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _eventPublisherService = eventPublisherService;
            _applicationExceptionHandler = applicationExceptionHandler;
            _contextAccessor = contextAccessor;
            _correlationService = correlationService;
        }
        #endregion

        #region Methods
        public async Task<LoginUserResponseDTO> LoginAsync(LoginCommand command)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(command.Email);

                if (existingUser.Details == null || string.IsNullOrEmpty(existingUser.Details.PasswordHash))
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                if (existingUser.Details.IsRegistrationConfirmed != 1 && existingUser.Details.IsActive != 1)
                {
                    throw new UnauthorizedAccessException("User has not yet confirmed registration");
                }

                bool isMatch = PasswordHasher.VerifyPassword(command.Password, existingUser.Details.PasswordHash);

                if (!isMatch)
                {
                    throw new UnauthorizedAccessException("Invalid password");
                }

                var user = await _userRepository.GetUserByCredentialsAsync(command.Email);

                if (user.Details == null || !user.ResultStatus)
                {
                    throw new UnauthorizedAccessException("Invalid credentials, check your email or password");
                }

                var role = await _roleRepository.GetRoleByIdAsync(user.Details.IdRole);

                if (role.Details == null || !role.ResultStatus)
                {
                    throw new InvalidOperationException($"Role with Id not found: {user.Details.IdRole}");
                }

                var userDetails = new LoginUserResponseDTO
                {
                    IdUser = user.Details.IdUser,
                    IdRole = user.Details.IdRole,
                    Role = RoleTypeExtension.GetRoleTypeName(user.Details.IdRole),
                    Email = user.Details.Email,
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Authorize.ToEntityName(),
                    operationType: AuditOperationType.Login.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: "User is logged in",
                    additionalData: userDetails,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Login_Success.ToRoutingKey()
                    );

                var token = _tokenService.GenerateAccessToken(userDetails);

                userDetails.Token = token;

                return userDetails;
            }
            catch (Exception ex)
            {
                var registerErrorEvent = new RegisterErrorEvent
                {
                    IdCorrelation = _correlationService.GetCorrelationId(),
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "UnknownLogger"
                };

                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Adapter, ActionType.Login);

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Authorize.ToEntityName(),
                    operationType: AuditOperationType.Login.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: registerErrorEvent,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Login_Error.ToRoutingKey()
                    );

                return null;
            }
        }
        #endregion
    }
}
