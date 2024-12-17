using SearchJobsService.Services.Interfaces;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.Publication;

namespace SearchJobsService.Services
{
    public class PublicationsService : IPublicationsService
    {
        #region Properties
        private readonly IAsyncEventBus _eventBus;
        #endregion

        #region Constructor
        public PublicationsService(IAsyncEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        #endregion

        #region Methods
        public async Task PublishCreatePublicationEvent(PublicationCreatedEvent publicationCreatedEvent)
        {
            await _eventBus.PublishAsyn("publication_exchange", "publication.created", publicationCreatedEvent);
        }

        public async Task PublishDeletePublicationEvent(int publicationId)
        {
            var deleteEvent = new { IdPublication = publicationId };
            await _eventBus.PublishAsyn("publication_exchange", "publication.deleted", deleteEvent);
        }
        #endregion
    }
}
