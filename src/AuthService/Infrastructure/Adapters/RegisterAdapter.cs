using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Services.Interfaces;
using SharedKernel.Common.Extensions;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Adapters
{
    public class RegisterAdapter : IRegisterPort
    {
        #region Properties
        private readonly IRegisterUserPort _registerRepository;
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public RegisterAdapter(
            IRegisterUserPort registerRepository,
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IGlobalExceptionHandler globalExceptionHandler
            )
        {
            _registerRepository = registerRepository;
            _userRepository = userRepository;
            _eventPublisherService = eventPublisherService;
            _globalExceptionHandler = globalExceptionHandler;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<RegisterUserDTO> RegisterAsync(RegisterCommand command)
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

                await _eventPublisherService.PublishEventAsyn(
                    entityName: "Authorize",
                    operationType: "Register",
                    success: true,
                    performedBy: response.ResultMessage,
                    additionalData: newUser,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Register_Success.ToRoutingKey()
                    );

                if (!response.ResultStatus && response.AffectedRecordId == 0)
                {
                    throw new Exception(response.ResultStatus.ToString());
                }

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
                _globalExceptionHandler.HandleGenericException<string>(ex, "RegisterAdapter");

                await _eventPublisherService.PublishEventAsyn(
                    entityName: "Authorize",
                    operationType: "Register",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Register_Error.ToRoutingKey()
                    );
                
                return null;
            }
        }
        #endregion
    }
}
