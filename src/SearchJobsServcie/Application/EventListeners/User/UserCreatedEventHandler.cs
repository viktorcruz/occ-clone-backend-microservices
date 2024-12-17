using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.User;

namespace SearchJobsService.Application.EventListeners.User
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
