using MediatR;
using SearchJobsService.Application.DTO.Commands;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;

namespace SearchJobsService.Application.Commands.Handler.Withdraw
{
    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public WithdrawCommandHandler(
            ISearchJobsDomain searchJobsDomain,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IHttpContextAccessor contextAccessor
            )
        {
            _searchJobsDomain = searchJobsDomain;
            _eventPublisherService = eventPublisherService;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = new WithdrawApplicationRequestDTO { IdUser = request.IdUser, IdPublication = request.IdPublication };
                var response = await _searchJobsDomain.WithdrawAsync(dto);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Withdraw successful";

                    var additionalData = new
                    {
                        IdApplicant = response.AffectedRecordId,
                        Message = response.ResultMessage
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.Job.ToEntityName(),
                        operationType: AuditOperationType.Withdraw.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Withdraw.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Withdraw);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Job.ToEntityName(),
                    operationType: AuditOperationType.Withdraw.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Withdraw_Failed.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
