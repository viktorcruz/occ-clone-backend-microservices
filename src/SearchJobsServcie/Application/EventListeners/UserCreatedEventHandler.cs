using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace SearchJobsService.Application.EventListeners
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        public async Task Handle(UserCreatedEvent @event)
        {
            // Implementa la lógica para manejar la creación de usuario en SearchJobsService.
            // Por ejemplo, crea un registro de búsqueda de empleo asociado al usuario.
        }
    }
}
