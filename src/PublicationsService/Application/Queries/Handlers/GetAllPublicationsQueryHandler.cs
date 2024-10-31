using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Domain.Events;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Queries.Handlers
{
    public class GetAllPublicationsQueryHandler : IRequestHandler<GetAllPulicationsQuery, IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public GetAllPublicationsQueryHandler(
            IPublicationDomain publicationDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> endpointResponse,
            IEventBus eventBus,
            IEntityOperationEventFactory entityOperationEventFactory
            )
        {
            _publicationDomain = publicationDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
            _entityOperationEventFactory = entityOperationEventFactory;
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

                    var eventInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "Publication",
                        operationType: "Get All",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("PublicationExchange", "PublicationGetAllEvent", eventInstance);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "Publications not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "GetAllUserQuery.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "GetAll", success: false, performedBy: "Admin");

                _eventBus.Publish("PublicationExchange", "PublicationGetAllEvent", failedEvent);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
