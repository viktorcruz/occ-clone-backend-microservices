using AuthService.Domain.Entities;
using SharedKernel.Common.Responses;
using SharedKernel.Events.User;

namespace AuthService.Domain.Ports.Output
{
    public interface IRegisterUserPort
    {
        Task<DatabaseResult> AddAsync(UserCreatedEvent createdEvent);
    }
}
