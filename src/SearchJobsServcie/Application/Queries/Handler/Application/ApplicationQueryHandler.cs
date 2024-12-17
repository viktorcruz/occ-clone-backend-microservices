using MediatR;
using SearchJobsService.Application.DTO.Queries;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Responses;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;

namespace SearchJobsService.Application.Queries.Handler.Application
{
    public class ApplicationQueryHandler : IRequestHandler<ApplicationQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public ApplicationQueryHandler(
            ISearchJobsDomain searchJobsDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> endpointResponse,
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
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>>> Handle(ApplicationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _searchJobsDomain.ApplicationsAsync(request.IdUser);
                _endpointResponse.Result = response;

                if (response == null || !response.ResultStatus)
                {
                    throw new Exception($"User not found: {request.IdUser}");
                }

                _endpointResponse.IsSuccess = true;
                _endpointResponse.Message = "Successful";

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Job.ToEntityName(),
                    operationType: AuditOperationType.Application.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: response.ResultMessage,
                    additionalData: response.Details,
                    exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Search_Success.ToRoutingKey()
                    );
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Application, ActionType.FetchAll);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error getting applications: {ex.Message}";
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Job.ToEntityName(),
                    operationType: AuditOperationType.Application.ToOperationType(),
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
