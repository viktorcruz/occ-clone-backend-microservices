using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace SearchJobsService.Application.EventListeners
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
