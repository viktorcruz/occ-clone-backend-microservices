using MediatR;
using SearchJobsService.Application.Dto;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Queries.Handler
{
    public class ApplicationQueryHandler : IRequestHandler<ApplicationQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public ApplicationQueryHandler(
            ISearchJobsDomain searchJobsDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> endpointResponse,
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
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>>> Handle(ApplicationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _searchJobsDomain.ApplicationsAsync(request.IdUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "SearchJobs",
                        operationType: "SEARCH",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage,
                        additionalData: response.Details,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Application not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Search Jobs",
                        operationType: "SEARCH",
                        success: false,
                        performedBy: "Admin",
                        reason: "Applicant not found",
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "ApplicationByIdQueryHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error getting applications: {ex.Message}";

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Application",
                    operationType: "SEARCH",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
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
