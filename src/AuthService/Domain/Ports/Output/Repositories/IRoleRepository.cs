using AuthService.Domain.Entities;
using SharedKernel.Common.Responses;

namespace AuthService.Domain.Ports.Output.Repositories
{
    public interface IRoleRepository
    {
        Task<RetrieveDatabaseResult<RoleEntity>> GetRoleByIdAsync(int roleId);
    }
}
