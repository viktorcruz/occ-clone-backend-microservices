using AuthService.Domain.Entities;
using SharedKernel.Common.Responses;

namespace AuthService.Domain.Ports.Output.Repositories
{
    public interface IUserRepository
    {
        Task<RetrieveDatabaseResult<UserByEmailEntity>> GetUserByCredentialsAsync(string email);
        Task<RetrieveDatabaseResult<UserByEmailEntity>> GetByEmailAsync(string email);
    }
}
