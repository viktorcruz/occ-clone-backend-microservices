using MediatR;
using SearchJobsService.Application.DTO;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Responses;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;

namespace SearchJobsService.Application.Queries.Handler.SearchJobs
{
    public class SearchJobsQueryHandler : IRequestHandler<SearchJobsQuery, IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public SearchJobsQueryHandler(
            ISearchJobsDomain searchJobsDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService,
            IHttpContextAccessor contextAccessor
            )
        {
            _searchJobsDomain = searchJobsDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>>> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Keyword))
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "At least one search criteria must be provide (Keyword, Location)";
                    return _endpointResponse;
                }

                var response = await _searchJobsDomain.SearchAsync(request.Keyword);

                if (response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Jobs retrieved successfully";
                    _endpointResponse.Result = response;

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.Job.ToEntityName(),
                        operationType: AuditOperationType.Search.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response?.ResultMessage ?? "Job not found",
                        additionalData: response?.Details,
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "Job not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Job",
                        operationType: "SEARCH",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Job not found",
                        additionalData: response?.Details ?? null,
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.FetchAll);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Job.ToEntityName(),
                    operationType: AuditOperationType.Search.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Search_Error.ToRoutingKey()
                );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
