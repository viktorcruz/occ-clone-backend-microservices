using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.User;

namespace SearchJobsService.Application.EventListeners.User
{
    public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
    {
        public UserUpdatedEventHandler()
        {

        }

        public Task Handle(UserUpdatedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
