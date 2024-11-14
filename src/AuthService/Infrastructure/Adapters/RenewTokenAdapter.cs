using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Adapters
{
    public class RenewTokenAdapter : IRenewTokenPort
    {
        #region Properties
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public RenewTokenAdapter(
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler
            )
        {
            _userRepository = userRepository;
            _eventPublisherService = eventPublisherService;
            _applicationExceptionHandler = applicationExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<RenewTokenResponseDTO> RenewAsync(string emailClaim)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(emailClaim);

                if (user.Details == null || !user.ResultStatus)
                {
                    return new RenewTokenResponseDTO
                    {
                        IdUser = 0,
                        Email = null
                    };
                }

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Authorize",
                    operationType: "Renew",
                    success: true,
                    performedBy: "Admin",
                    additionalData: new { IdUser = user.Details.IdUser, Email = user.Details.Email },
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Renew_Success.ToRoutingKey()
                    );

                return new RenewTokenResponseDTO
                {
                    IdUser = user.Details.IdUser,
                    Email = user.Details.Email,
                };
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Adapter, ActionType.Renew);
                var registerErrorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: "Authorize",
                    operationType: "Renew",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: registerErrorEvent,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Renew_Error.ToRoutingKey()
                    );

                return null;
            }
        }
        #endregion
    }
}
