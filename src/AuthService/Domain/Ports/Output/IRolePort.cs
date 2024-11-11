using AuthService.Domain.Entities;
using SharedKernel.Common.Responses;

namespace AuthService.Domain.Ports.Output
{
    public interface IRolePort
    {
        Task<RetrieveDatabaseResult<RoleEntity>> GetRoleByIdAsync(int roleId);
    }
}
