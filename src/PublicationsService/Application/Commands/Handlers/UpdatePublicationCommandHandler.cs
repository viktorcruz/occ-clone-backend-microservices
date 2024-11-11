using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Application.Dto;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands.Handlers
{
    public class UpdatePublicationCommandHandler : IRequestHandler<UpdatePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public UpdatePublicationCommandHandler(
            IPublicationDomain publicationDomain,
            IEventPublisherService eventPublisherService,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventBus eventBus,
            IEntityOperationEventFactory entityOperationEventFactory
            )
        {
            _publicationDomain = publicationDomain;
            _eventPublisherService = eventPublisherService;
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

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Publication updated successful";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "UPDATE",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Publication not found",
                        additionalData: publication,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "UPDATE",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Publication not found",
                        additionalData: publication,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Update_Error.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UpdatePublicationCommand.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating publication: {ex.Message}";

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Publication",
                    operationType: "UPDATE",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
