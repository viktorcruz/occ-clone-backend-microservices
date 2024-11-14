using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace UsersService.Application.EventListeners
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        public Task Handle(UserCreatedEvent @event)
        {
            Console.WriteLine($"User created: {@event.IdUser}, email: {@event.Email}");
            // Lógica adicional para el evento
            return Task.CompletedTask;
        }
    }
}
