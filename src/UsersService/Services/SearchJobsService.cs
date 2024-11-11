using SharedKernel.Common.Interfaces;
using UsersService.Services.Interfaces;

namespace UsersService.Services
{
    public class SearchJobsService : ISearchJobsService
    {
        #region Properties
        private readonly IAsyncEventBus _eventBus;
        #endregion

        #region Constructor
        public SearchJobsService(IAsyncEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        #endregion

        #region Methods
        public async Task PublishRevertJobSearchUpdateEvent(int userId)
        {
            var revertEvent = new {IdUser = userId};
            await _eventBus.PublishAsyn("search_jobs_exchange", "searchjobs.revert", revertEvent);
        }

        public async Task PublishUpdateJobSearchevent(int userId, int publicationId)
        {
            var updatedEvent = new {IdUser = userId, IdPublication = publicationId};
            await _eventBus.PublishAsyn("search_jobs_exchange", "searchjobs.revert", updatedEvent);
        }
        #endregion
    }
}
