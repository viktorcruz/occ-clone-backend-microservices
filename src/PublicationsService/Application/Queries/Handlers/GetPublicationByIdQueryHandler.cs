using Azure;
using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Domain.Events;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Queries.Handlers
{
    public class GetPublicationByIdQueryHandler : IRequestHandler<GetPublicationByIdQuery, IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public GetPublicationByIdQueryHandler(
            IPublicationDomain publicationDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>> endpointResponse,
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
        public async Task<IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>>> Handle(GetPublicationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.GetPublicationByIdAsync(request.IdPublication);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        IdPublication = request.IdPublication,
                        IdUser = response.Details.IdUser,
                        IdRole = response.Details.IdRole,
                        Title = response.Details.Title,
                        Description = response.Details.Description,
                        PublicationDate = response.Details.PublicationDate,
                        ExpirationDate = response.Details.ExpirationDate,
                        Status = response.Details.Status,
                        Location = response.Details.Location,
                        Company = response.Details.Company,
                    };

                    var entityInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "Publication",
                        operationType: "Get By Id",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultMessage,
                        additionalData: additionalData
                        );

                    _eventBus.Publish("PublicationExchange", "GetByIdPublication", entityInstance);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "GetByIdPublicationQueryHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error getting publication: {ex.Message}";

                var failedEvent = _entityOperationEventFactory.CreateEvent(
                        entityName: "Publication",
                        operationType: "Get By Id",
                        success: false,
                        performedBy: "Admin",
                        reason: ex.Message,
                        additionalData: null
                    );
                _eventBus.Publish("PublicationExchange", "GetByIdPublicationFailed", failedEvent);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
