using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.Publication;

namespace SearchJobsService.Application.EventListeners.Publication
{
    public class PublicationCreationFailedEventHandler : IEventHandler<PublicationCreationFailedEvent>
    {
        public Task Handle(PublicationCreationFailedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
