using MediatR;
using PublicationsService.Domain.Interface;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands.Handlers
{
    public class DeletePublicationCommandHandler : IRequestHandler<DeletePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public DeletePublicationCommandHandler(
            IPublicationDomain publicationDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventBus eventBus,
            IEntityOperationEventFactory entityOperationEventFactory
            )
        {
            _publicationDomain = publicationDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _entityOperationEventFactory = entityOperationEventFactory;
            _eventBus = eventBus;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(DeletePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.DeletePublicationByIdAsync(request.IdPublication);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Publication deleted successful";

                    var additionalData = new
                    {
                        IdPublication = request.IdPublication,
                        IsDeleted = true
                    };

                    var entityInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "Publication",
                        operationType: "Delete",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultMessage,
                        additionalData: additionalData
                        );

                    _eventBus.Publish("PublicationExchange", "PublicationDeletedEvent", entityInstance);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "DeletePublicationCommand.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error deleting publication: {ex.Message}";

                var failedEvent = _entityOperationEventFactory.CreateEvent(
                    entityName: "Publication",
                        operationType: "Delete",
                        success: false,
                        performedBy: "Admin",
                        reason: ex.Message,
                        additionalData: null
                        );

                _eventBus.Publish("PublicationExchange", "PublicationDeletedEvent", _entityOperationEventFactory);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
