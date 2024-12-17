using SharedKernel.Events.Publication;

namespace SearchJobsService.Services.Interfaces
{
    public interface IPublicationsService
    {
        Task PublishCreatePublicationEvent(PublicationCreatedEvent publicationCreatedEvent);
        Task PublishDeletePublicationEvent(int publicationId);  
    }
}
