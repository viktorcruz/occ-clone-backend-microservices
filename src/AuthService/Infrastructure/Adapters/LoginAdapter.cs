using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using AuthService.Domain.Ports.Output.Services;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Services.Interfaces;
using SharedKernel.Common.Extensions;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Adapters
{
    public class LoginAdapter : ILoginPort
    {
        #region Properties
        private readonly IUserPort _userRepository;
        private readonly IRolePort _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public LoginAdapter(
            IUserPort userRepository,
            IRolePort roleRepository,
            ITokenService tokenService,
            IEventPublisherService eventPublisherService,
            IGlobalExceptionHandler globalExceptionHandler
            )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _eventPublisherService = eventPublisherService;
            _globalExceptionHandler = globalExceptionHandler;
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
                    Email = user.Details.Email,
                };

                await _eventPublisherService.PublishEventAsyn(
                    entityName: "Auhorize",
                    operationType: "Login",
                    success: true,
                    performedBy: "Admin",
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
                _globalExceptionHandler.HandleGenericException<string>(ex, "LoginAdapter");

                await _eventPublisherService.PublishEventAsyn(
                    entityName: "Authorize",
                    operationType: "Login",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Login_Error.ToRoutingKey()
                    );

                return null;
            }
        }
        #endregion
    }
}
