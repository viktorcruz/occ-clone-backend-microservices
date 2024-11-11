using SharedKernel.Common.Events;

namespace UsersService.Services.Interfaces
{
    public interface IPublicationsService
    {
        Task PublishCreatePublicationEvent(PublicationCreatedEvent publicationCreatedEvent);
        Task PublishDeletePublicationEvent(int publicationId);
    }
}
