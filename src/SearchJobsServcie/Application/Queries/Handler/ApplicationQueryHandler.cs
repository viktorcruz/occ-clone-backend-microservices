﻿using MediatR;
using SearchJobsService.Application.Dto;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Queries.Handler
{
    public class ApplicationQueryHandler : IRequestHandler<ApplicationQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public ApplicationQueryHandler(
            ISearchJobsDomain searchJobsDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _searchJobsDomain = searchJobsDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
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
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
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
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Search_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.FetchAll);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error getting applications: {ex.Message}";
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: "Application",
                    operationType: "SEARCH",
                    success: false,
                    performedBy: "Admin",
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
