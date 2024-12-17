using MediatR;
using PublicationsService.Aplication.DTO;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Responses;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;

namespace PublicationsService.Aplication.Queries.Handlers
{
    public class GetAllPublicationsQueryHandler : IRequestHandler<GetAllPulicationsQuery, IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public GetAllPublicationsQueryHandler(
            IPublicationDomain publicationDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService,
            IHttpContextAccessor contextAccessor
            )
        {
            _publicationDomain = publicationDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>>> Handle(GetAllPulicationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.GetAllPublicationAsync();
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        TotalPublications = response.Details.Count,
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.Publication.ToEntityName(),
                        operationType: AuditOperationType.GetAll.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response?.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.GetAll_Success.ToRoutingKey()
                        );
                }
                //else
                //{
                //    _endpointResponse.IsSuccess = false;
                //    _endpointResponse.Message = "Publications not found";

                //    await _eventPublisherService.PublishEventAsync(
                //        entityName: "Publication",
                //        operationType: "GETALL",
                //        success: false,
                //        performedBy: "Admin",
                //        reason: response?.ResultMessage ?? "Publications not found",
                //        additionalData: null,
                //        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                //        routingKey: PublicationRoutingKeys.GetAll_Failed.ToRoutingKey()
                //    );
                //}
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.FetchAll);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Publication.ToEntityName(),
                    operationType: AuditOperationType.GetAll.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.GetAll_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
