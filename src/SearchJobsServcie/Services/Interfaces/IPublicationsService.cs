using SharedKernel.Common.Events;

namespace SearchJobsService.Services.Interfaces
{
    public interface IPublicationsService
    {
        Task PublishCreatePublicationEvent(PublicationCreatedEvent publicationCreatedEvent);
        Task PublishDeletePublicationEvent(int publicationId);  
    }
}
