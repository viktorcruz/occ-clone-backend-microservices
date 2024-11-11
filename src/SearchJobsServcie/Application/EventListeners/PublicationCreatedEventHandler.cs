using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace SearchJobsService.Application.EventListeners
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
