using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.User;

namespace PublicationsService.Application.EventListeners
{
    public class UserDeletedEventHandler : IEventHandler<UserDeletedEvent>
    {

        public Task Handle(UserDeletedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
