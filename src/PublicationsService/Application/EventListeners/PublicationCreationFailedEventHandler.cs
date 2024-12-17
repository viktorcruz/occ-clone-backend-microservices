using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.Publication;

namespace PublicationsService.Application.EventListeners
{
    public class PublicationCreationFailedEventHandler : IEventHandler<PublicationCreationFailedEvent>
    {
        public Task Handle(PublicationCreationFailedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
