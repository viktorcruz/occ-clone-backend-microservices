using MediatR;
using SearchJobsService.Application.Dto;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Commands.Handler
{
    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        #endregion

        #region Constructor
        public WithdrawCommandHandler(
            ISearchJobsDomain searchJobsDomain,
            IEventPublisherService eventPublisherService,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse
            )
        {
            _searchJobsDomain = searchJobsDomain;
            _eventPublisherService = eventPublisherService;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
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
                        entityName: "Withdraw",
                        operationType: "WITHDRAW",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Withdraw_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "User not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Withdraw",
                        operationType: "WITHDRAW",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "User not found",
                        additionalData: request,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Withdraw_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "WithdrawCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Withdraw",
                    operationType: "WITHDRAW",
                    success: true,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: request,
                    exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Withdraw_Failed.ToRoutingKey()
                    //"search_jobs_exchange",
                    //"search_jobs.withdraw.failed"
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
