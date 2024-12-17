using SharedKernel.Events.User;

namespace PublicationsService.Services.Interfaces
{
    public interface IUsersService
    {
        Task PublishCreateUserEvent(UserCreatedEvent userCreatedEvent);
        Task PuslishDeleteUserEvent(int userId);    
    }
}
