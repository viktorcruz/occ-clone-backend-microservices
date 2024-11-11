using PublicationsService.Services.Interfaces;
using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace PublicationsService.Services
{
    public class UsersService : IUsersService
    {
        #region Properties
        private readonly IAsyncEventBus _eventBus;
        #endregion

        #region Constructor
        public UsersService(IAsyncEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        #endregion

        #region Methods
        public async Task PublishCreateUserEvent(UserCreatedEvent userCreatedEvent)
        {
            await _eventBus.PublishAsyn("user_exchange", "user.created", userCreatedEvent);
        }

        public async Task PuslishDeleteUserEvent(int userId)
        {
            var deleteEvent = new { IdUser = userId };
            await _eventBus.PublishAsyn("user_exchange", "user.deleted", deleteEvent);
        }
        #endregion
    }
}
