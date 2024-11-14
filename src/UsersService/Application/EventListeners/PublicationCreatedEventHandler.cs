using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace UsersService.Application.EventListeners
{
    public class PublicationCreatedEventHandler : IEventHandler<PublicationCreatedEvent>
    {
        public Task Handle(PublicationCreatedEvent @event)
        {
            Console.WriteLine($"Publication created: {@event.IdPublication} user: {@event.IdUser}");
            return Task.CompletedTask;
        }
    }
}
