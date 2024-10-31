using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Application.Dto;
using PublicationsService.Domain.Interface;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands.Handlers
{
    public class UpdatePublicationCommandHandler : IRequestHandler<UpdatePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public UpdatePublicationCommandHandler(
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
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(UpdatePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var publication = new PublicationUpdateDTO
                {
                    IdPublication = request.IdPubliaction,
                    IdUser = request.IdUser,
                    IdRole = request.IdRole,
                    Title = request.Title,
                    Description = request.Description,
                    Status = request.Status,
                    Salary = request.Salary,
                    Location = request.Location,
                    Company = request.Company,
                };

                var response = await _publicationDomain.UpdatePublicationAsync(publication);
                _endpointResponse.Result = response;

                if(response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Publication updated successful";

                    var entityInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "Publication",
                        operationType: "Update",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: publication
                        );
                    _eventBus.Publish("PublicationExchange", "PublicationUpdateEvent", entityInstance);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UpdatePublicationCommand.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating publication: {ex.Message}";

                var failedEvent = _entityOperationEventFactory.CreateEvent(
                    entityName: "Publication",
                    operationType: "Update",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null
                    );
                _eventBus.Publish("PublicationExchange", "PublicationUpdateEventFailed", failedEvent);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
