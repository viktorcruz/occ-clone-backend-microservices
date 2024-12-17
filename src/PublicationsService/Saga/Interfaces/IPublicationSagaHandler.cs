using SharedKernel.Events.Auth;

namespace PublicationsService.Saga.Interfaces
{
    public interface IPublicationSagaHandler
    {
        Task Handle(RegisterSuccessEvent registerEvent);
        Task Handle(RegisterErrorEvent errorEvent);
    }
}
