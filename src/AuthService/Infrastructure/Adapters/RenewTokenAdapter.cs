using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output;
using AuthService.Infrastructure.Services.Interfaces;
using SharedKernel.Common.Extensions;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Adapters
{
    public class RenewTokenAdapter : IRenewTokenPort
    {
        #region Properties
        private readonly IUserPort _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public RenewTokenAdapter(
            IUserPort userRepository,
            IEventPublisherService eventPublisherService,
            IGlobalExceptionHandler globalExceptionHandler
            )
        {
            _userRepository = userRepository;
            _eventPublisherService = eventPublisherService;
            _globalExceptionHandler = globalExceptionHandler;
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

                await _eventPublisherService.PublishEventAsyn(
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
                _globalExceptionHandler.HandleGenericException<string>(ex, "RenewAdapter");

                await _eventPublisherService.PublishEventAsyn(
                    entityName: "Authorize",
                    operationType: "Renew",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null,
                    exchangeName: PublicationExchangeNames.Authorize.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Renew_Error.ToRoutingKey()
                    );

                return null;
            }
        }
        #endregion
    }
}
