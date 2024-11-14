using PublicationsService.Application.EventListeners;
using SharedKernel.Common.Interfaces;

namespace SearchJobsService.Application.EventListeners
{
    public class PublicationCreationFailedEventHandler : IEventHandler<PublicationCreationFailedEvent>
    {
        public Task Handle(PublicationCreationFailedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
