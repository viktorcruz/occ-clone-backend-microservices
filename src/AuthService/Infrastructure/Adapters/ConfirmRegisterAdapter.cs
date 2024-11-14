using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Adapters
{
    public class ConfirmRegisterAdapter : IConfirmRegisterPort
    {
        #region Properties
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;

        #endregion

        #region Constructor
        public ConfirmRegisterAdapter(
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler
            )
        {
            _userRepository = userRepository;
            _applicationExceptionHandler = applicationExceptionHandler;
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

                await _eventPublisherService.PublishEventAsync(
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
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Adapter, ActionType.Update);
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: "Auhotize",
                    operationType: "Login",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Confirmation_Error.ToRoutingKey()
                    );

                return null;
            }
        }
        #endregion
    }
}
