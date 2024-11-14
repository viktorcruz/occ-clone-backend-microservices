using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace SearchJobsService.Application.EventListeners
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
