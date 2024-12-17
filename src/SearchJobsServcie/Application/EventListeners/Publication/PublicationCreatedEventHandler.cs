using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.Publication;

namespace SearchJobsService.Application.EventListeners.Publication
{
    public class PublicationCreatedEventHandler : IEventHandler<PublicationCreatedEvent>
    {
        public async Task Handle(PublicationCreatedEvent @event)
        {
            // Implementa la lógica para manejar el evento de creación de publicación
            // en el contexto del servicio de búsqueda, como indexar la publicación.
        }
    }
}
