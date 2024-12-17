using MediatR;
using PublicationsService.Aplication.Commands;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Publication;
using SharedKernel.Extensions.Routing;

namespace PublicationsService.Saga
{
    public class SagaOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly ILogger<SagaOrchestrator> _logger;

        public SagaOrchestrator(
            IMediator mediator,
            IEventPublisherService eventPublisherService,
            ILogger<SagaOrchestrator> logger)
        {
            _mediator = mediator;
            _eventPublisherService = eventPublisherService;
            _logger = logger;
        }

        public async Task ExecuteAsync(CreatePublicationCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("[SagaOrchestrator] Starting saga for creating publication.");

                // Paso 1: Crear la publicación
                var publicationResponse = await _mediator.Send(command, cancellationToken);

                if (!publicationResponse.IsSuccess)
                {
                    throw new Exception("Failed to create publication");
                }

                _logger.LogInformation("[SagaOrchestrator] Publication created successfully.");

                // Publicar un evento de éxito
                var successEvent = new PublicationCreatedEvent
                {
                    IdUser = command.IdUser,
                    IdRole = command.IdRole,
                    Title = command.Title,
                    Description = command.Description,
                    CreatedAt = DateTime.UtcNow
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Publication",
                    operationType: "CREATE",
                    success: true,
                    performedBy: "Admin",
                    reason: "Publication created successfully.",
                    additionalData: successEvent,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Create_Success.ToRoutingKey());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SagaOrchestrator] Saga failed. Triggering compensation.");

                // Publicar un evento de fallo
                var failedEvent = new PublicationFailedEvent
                {
                    IdUser = command.IdUser,
                    IdRole = command.IdRole,
                    Title = command.Title,
                    Reason = ex.Message,
                    FailedAt = DateTime.UtcNow
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Publication",
                    operationType: "CREATE",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: failedEvent,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Create_Error.ToRoutingKey());

                throw; // Re-lanzar la excepción
            }
        }
    }

}
