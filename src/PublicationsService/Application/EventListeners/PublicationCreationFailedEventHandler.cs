using SharedKernel.Common.Interfaces;

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
