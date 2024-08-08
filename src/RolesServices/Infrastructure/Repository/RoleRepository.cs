using RolesServices.Aplication.Dto;
using RolesServices.Infrastructure.Interface;
using SharedKernel.Common.Responses;

namespace RolesServices.Infrastructure.Repository
{
    public class RoleRepository : IRoleRespository
    {
        public Task<DatabaseResult> CreateRoleAsync(RoleDTO roleDTO)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResult> DeleteRoleAsync(int roleId)
        {
            throw new NotImplementedException();
        }

        public Task<RetrieveDatabaseResult<List<RoleDTO>>> GetAllRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RetrieveDatabaseResult<RoleDTO>> GetRoleByIdAsync(int roleId)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResult> UpdateRoleAsync(RoleDTO roleDTO)
        {
            throw new NotImplementedException();
        }
    }
}
