using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace UsersService.Application.EventListeners
{
    public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
    {
        public Task Handle(UserUpdatedEvent @event)
        {
            Console.WriteLine($"User updated: {@event.UserId}, email: {@event.Email}");
            // Lógica adicional para el evento
            return Task.CompletedTask;
        }
    }
}
