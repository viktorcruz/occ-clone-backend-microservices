using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.User;

namespace UsersService.Application.EventListeners
{
    public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
    {
        public Task Handle(UserUpdatedEvent @event)
        {
            Console.WriteLine($"User updated: {@event.IdUser}, email: {@event.Email}");
            // Lógica adicional para el evento
            return Task.CompletedTask;
        }
    }
}
