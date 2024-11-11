using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace PublicationsService.Application.EventListeners
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        public async Task Handle(UserCreatedEvent @event)
        {
            // Implementa la lógica para manejar el evento de creación de usuario
            // en el contexto de publicaciones, como crear una publicación inicial
            // o registrar al usuario en el sistema de publicaciones.
        }
    }
}
