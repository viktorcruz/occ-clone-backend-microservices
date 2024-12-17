using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.User;

namespace UsersService.Events.Handlers
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        public async Task Handle(UserCreatedEvent @event)
        {
            // Agrega logs aquí para confirmar si se llama al handler
            Console.WriteLine("    -=[*******]=-    UserCreatedEventHandler invocado");
            Console.WriteLine($"    -=[*******]=-    IdUser: {@event.IdUser} - {@event.Email}");

            await Task.CompletedTask; // Evitar posibles problemas con métodos no asincrónicos
        }
    }
}
