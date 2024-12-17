using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.Auth;

namespace UsersService.Events.Handlers
{
    public class RegisterSuccessEventHandler : IEventHandler<RegisterSuccessEvent>
    {
        public Task Handle(RegisterSuccessEvent @event)
        {
            Console.WriteLine("RegisterSuccessEvent recibido en UserService");
            Console.WriteLine($"IdUser: {@event.IdUser}");

            return Task.CompletedTask;
        }
    }
}
