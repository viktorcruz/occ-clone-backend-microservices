using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

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
