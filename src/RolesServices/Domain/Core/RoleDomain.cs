using RolesServices.Aplication.Dto;
using RolesServices.Domain.Interface;
using RolesServices.Infrastructure.Interface;
using SharedKernel.Common.Responses;

namespace RolesServices.Domain.Core
{
    public class RoleDomain : IRoleDomain
    {
        #region Properties
        public readonly IRoleRespository _roleRepository;
        #endregion

        #region Methods
        public RoleDomain(IRoleRespository userRepository)
        {
            _roleRepository = userRepository;
        }

        public async Task<DatabaseResult> CreateRoleAsync(RoleDTO roleDTO)
        {
            return await _roleRepository.CreateRoleAsync(roleDTO);
        }

        public async Task<RetrieveDatabaseResult<RoleDTO>> GetRoleByIdAsync(int roleId)
        {
            return await _roleRepository.GetRoleByIdAsync(roleId);
        }

        public async Task<RetrieveDatabaseResult<List<RoleDTO>>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<DatabaseResult> UpdateRoleAsync(RoleDTO roleDTO)
        {
            return await _roleRepository.UpdateRoleAsync(roleDTO);
        }

        public async Task<DatabaseResult> DeleteRoleAsync(int roleId)
        {
            return await _roleRepository.DeleteRoleAsync(roleId);
        }
        #endregion
    }
}
