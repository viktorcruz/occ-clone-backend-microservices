using AuthService.Domain.Entities;
using SharedKernel.Common.Responses;

namespace AuthService.Domain.Ports.Output
{
    public interface IUserPort
    {
        Task<RetrieveDatabaseResult<UserByEmailEntity>> GetUserByCredentialsAsync(string email);
        Task<RetrieveDatabaseResult<UserByEmailEntity>> GetByEmailAsync(string email);
        Task<DatabaseResult> ChangeUserStatusAsync(int userId, string email);
    }
}
