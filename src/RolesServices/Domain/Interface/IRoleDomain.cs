using RolesServices.Aplication.Dto;
using SharedKernel.Common.Responses;

namespace RolesServices.Domain.Interface
{
    public interface IRoleDomain
    {
        Task<DatabaseResult> CreateRoleAsync(RoleDTO roleDTO);
        Task<RetrieveDatabaseResult<RoleDTO>> GetRoleByIdAsync(int roleId);
        Task<RetrieveDatabaseResult<List<RoleDTO>>> GetAllRolesAsync();
        Task<DatabaseResult> UpdateRoleAsync(RoleDTO roleDTO);
        Task<DatabaseResult> DeleteRoleAsync(int roleId);
    }
}
