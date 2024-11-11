using MediatR;
using SearchJobsService.Application.Dto;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Queries.Handler
{
    public class SearchJobsQueryHandler : IRequestHandler<SearchJobsQuery, IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public SearchJobsQueryHandler(
            ISearchJobsDomain searchJobsDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _searchJobsDomain = searchJobsDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
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
                        entityName: "Search Jobs",
                        operationType: "SEARCH",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Job not found",
                        additionalData: response?.Details,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "Job not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "SearchJobs",
                        operationType: "SEARCH",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Job not found",
                        additionalData: response?.Details ?? null,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "SearchJobsQueryHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                await _eventPublisherService.PublishEventAsync(
                        entityName: "SearchJobs",
                        operationType: "SEARCH",
                        success: false,
                        performedBy: "Admin",
                        reason: "Job not found",
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
