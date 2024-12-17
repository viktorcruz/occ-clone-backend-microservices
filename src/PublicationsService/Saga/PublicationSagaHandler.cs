using PublicationsService.Saga.Interfaces;
using SharedKernel.Events.Auth;

namespace PublicationsService.Saga
{
    public class PublicationSagaHandler : IPublicationSagaHandler
    {
        public Task Handle(RegisterSuccessEvent registerEvent)
        {
            throw new NotImplementedException();
        }

        public Task Handle(RegisterErrorEvent errorEvent)
        {
            throw new NotImplementedException();
        }
    }
}
