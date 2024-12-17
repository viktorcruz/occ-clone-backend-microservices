using SharedKernel.Events.Auth;

namespace UsersService.Saga.Interfaces
{
    public interface IUserSagaHandler
    {
        Task Handle(RegisterSuccessEvent registerEvent);
        Task Handle(RegisterErrorEvent errorEvent);
    }
}
