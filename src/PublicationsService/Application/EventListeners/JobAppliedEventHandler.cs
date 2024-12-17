using PublicationsService.Application.DTO;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Enum;
using SharedKernel.Events.JobSearch;
using SharedKernel.Events.Publication;
using SharedKernel.Extensions.Routing;

namespace PublicationsService.Application.EventListeners
{
    public class JobAppliedEventHandler : IEventHandler<JobAppliedEvent>
    {
        private readonly IPublicationDomain _publicationDomain;
        private readonly IEventPublisherService _eventPublisherService;

        public JobAppliedEventHandler(IPublicationDomain publicationDomain, IEventPublisherService eventPublisherService)
        {
            _publicationDomain = publicationDomain;
            _eventPublisherService = eventPublisherService;
        }

        public async Task Handle(JobAppliedEvent @event)
        {
            try
            {
                Console.WriteLine($"[PublicationService] Updating publication for JobId={@event.IdJob}");

                // Construir el DTO con los datos necesarios para la actualización completa
                var publicationUpdate = new PublicationUpdateDTO
                {
                    IdPublication = @event.IdJob,
                    IdUser = @event.IdUser, // Relación de quién aplica
                    Title = "Updated Job Title", // Otros campos pueden mantenerse como valores existentes
                    Description = "Updated Job Description",
                    Status = (int)RecruitmentStatus.InReview,
                    Salary = 50000, // Ejemplo: datos existentes o calculados
                    Location = "Remote",
                    Company = "Company Name"
                };

                // Realizar la actualización completa
                var response = await _publicationDomain.UpdatePublicationAsync(publicationUpdate);

                if (response == null || !response.ResultStatus)
                {
                    throw new Exception($"Publication not found: {@event.IdJob}");
                }

                Console.WriteLine("[PublicationService] Successfully updated publication.");

                // Publicar un evento indicando que la publicación se actualizó
                var publicationUpdatedEvent = new PublicationUpdatedEvent
                {
                    IdPublication = @event.IdJob,
                    UpdatedAt = DateTime.UtcNow
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Publication",
                    operationType: "UPDATE",
                    success: true,
                    performedBy: "System",
                    reason: "Publication updated successfully",
                    additionalData: publicationUpdatedEvent,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PublicationService] Error updating publication: {ex.Message}");

                // Publicar evento de compensación
                var failedEvent = new JobApplicationFailedEvent
                {
                    IdJob = @event.IdJob,
                    IdApplicant = @event.IdUser,
                    Reason = ex.Message,
                    FailedAt = DateTime.UtcNow
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Job",
                    operationType: "APPLY",
                    success: false,
                    performedBy: "System",
                    reason: ex.Message,
                    additionalData: failedEvent,
                    exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Apply_Error.ToRoutingKey());
            }
        }
    }
}
