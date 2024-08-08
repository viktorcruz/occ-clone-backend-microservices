using RolesServices.Aplication.Dto;
using SharedKernel.Common.Responses;

namespace RolesServices.Infrastructure.Interface
{
    public interface IRoleRespository
    {
        Task<DatabaseResult> CreateRoleAsync(RoleDTO roleDTO);
        Task<RetrieveDatabaseResult<RoleDTO>> GetRoleByIdAsync(int roleId);
        Task<RetrieveDatabaseResult<List<RoleDTO>>> GetAllRolesAsync();
        Task<DatabaseResult> UpdateRoleAsync(RoleDTO roleDTO);
        Task<DatabaseResult> DeleteRoleAsync(int roleId);
    }
}
