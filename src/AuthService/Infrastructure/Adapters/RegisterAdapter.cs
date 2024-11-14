using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output;
using AuthService.Infrastructure.Security;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Adapters
{
    public class RegisterAdapter : IRegisterPort
    {
        #region Properties
        private readonly IRegisterUserPort _registerRepository;
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public RegisterAdapter(
            IRegisterUserPort registerRepository,
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler
            )
        {
            _registerRepository = registerRepository;
            _userRepository = userRepository;
            _eventPublisherService = eventPublisherService;
            _applicationExceptionHandler = applicationExceptionHandler;
            _applicationExceptionHandler = applicationExceptionHandler;
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

                var newUser = new RegisterEntity
                {
                    IdRole = command.IdRole,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                    PasswordHash = hashedPassword
                };

                var response = await _registerRepository.AddAsync(newUser);

                if (!response.ResultStatus && response.AffectedRecordId == 0)
                {
                    throw new Exception(response.ExceptionMessage);
                }

                newUser.IdUser = response.AffectedRecordId;

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Authorize",
                    operationType: "Register",
                    success: true,
                    performedBy: "Admin",
                    additionalData: newUser,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Register_Success.ToRoutingKey()
                    );

                var userResponse = new RegisterUserDTO
                {
                    IdRole = newUser.IdRole,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = newUser.Email,
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

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Authorize",
                    operationType: "Register",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: registerErrorEvent,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Register_Error.ToRoutingKey()
                    );

                return null;
            }
        }
        #endregion
    }
}
