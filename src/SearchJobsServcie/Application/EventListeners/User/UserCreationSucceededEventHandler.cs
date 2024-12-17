using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.User;

namespace SearchJobsService.Application.EventListeners.User
{
    public class UserCreationSucceededEventHandler : IEventHandler<UserCreationSucceededEvent>
    {
        public async Task Handle(UserCreationSucceededEvent @event)
        {
            // Implementa la lógica para manejar el éxito de la creación de usuario en SearchJobsService.
            // Por ejemplo, inicia un proceso de indexación o actualización de perfil.
        }
    }
}
