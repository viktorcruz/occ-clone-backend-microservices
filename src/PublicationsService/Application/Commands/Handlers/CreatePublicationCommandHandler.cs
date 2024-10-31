using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Domain.Events;
using PublicationsService.Domain.Interface;
using PublicationsService.Infrastructure.Messaging;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands.Handlers
{
    public class CreatePublicationCommandHandler : IRequestHandler<CreatePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public CreatePublicationCommandHandler(
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
            _eventBus = eventBus;
            _entityOperationEventFactory = entityOperationEventFactory;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(CreatePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.CreatePublicationAsync(request);

                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Publication created successful";

                    var additionalData = new
                    {
                        IdUser = request.IdUser,
                        IdRole = request.IdRole,
                        Title = request.Title,
                        Description = request.Description,
                        ExpirationDate = request.ExpirationDate,
                        Status = request.Status,
                        Salary = request.Salary,
                        Location = request.Location,
                    };

                    var eventInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "Publication",
                        operationType: "Create",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("PublicationExchange", "PublicationCreated", eventInstance);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not ncreated";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "CreatePublicationCommandHandler");

                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error creating publication: {ex.Message}";

                var failedEvent = new EntityOperationEvent(
                    entityName: "Publication",
                    operationType: "Create",
                    success: false,
                    performedBy: "AdminUser",
                    reason: ex.Message,
                    additionalData: null
                    );

                _eventBus.Publish("PublicationExchance", "PublicationCreateFailed", failedEvent);
            }

            return _endpointResponse;
        }
        #endregion
    }
}
