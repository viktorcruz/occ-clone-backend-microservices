using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using AuthService.Infrastructure.Services.Interfaces;
using SharedKernel.Common.Extensions;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Adapters
{
    public class ConfirmRegisterAdapter : IConfirmRegisterPort
    {
        #region Properties
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;

        #endregion

        #region Constructor
        public ConfirmRegisterAdapter(
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IGlobalExceptionHandler globalExceptionHandler
            )
        {
            _userRepository = userRepository;
            _globalExceptionHandler = globalExceptionHandler;
            _eventPublisherService = eventPublisherService;
        }
        #endregion

        #region Methods
        public async Task<ConfirmRegisterResponseDTO> ConfirmRegisterAsync(ConfirmRegisterCommand command)
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
                    RegistrationConfirmationDescription = "User registration is confirmed"
                };

                await _eventPublisherService.PublishEventAsyn(
                    entityName: "Authorize",
                    operationType: "Login",
                    success: true,
                    performedBy: "Admin",
                    additionalData: response,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Confirmation_Success.ToRoutingKey()
                    );

                return response;
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "ConfirmRegisterAdapter");

                await _eventPublisherService.PublishEventAsyn(
                    entityName: "Auhotize",
                    operationType: "Login",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: ex.StackTrace,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Confirmation_Error.ToRoutingKey()
                    );

                return null;
            }
        }
        #endregion
    }
}
