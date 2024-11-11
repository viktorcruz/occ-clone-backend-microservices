using SharedKernel.Common.Events;

namespace PublicationsService.Services.Interfaces
{
    public interface IUsersService
    {
        Task PublishCreateUserEvent(UserCreatedEvent userCreatedEvent);
        Task PuslishDeleteUserEvent(int userId);    
    }
}
