using UsersService.Application.Dto;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Interface;
using UsersService.SharedKernel.Common.Response;

namespace UsersService.Domain.Core
{
    public class UsersDomain : IUsersDomain
    {
        #region Properties
        public readonly IUserRepository _userRepository;
        #endregion

        #region Methods
        public UsersDomain(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<SpResult> CreateUserAsync(AddUserDTO userDTO)
        {
            return await _userRepository.CreateUserAsync(userDTO);
        }

        public async Task<SpRetrieveResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<SpRetrieveResult<List<UserRetrieveDTO>>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<SpResult> UpdateUserAsync(UserRetrieveDTO userDTO)
        {
            return await _userRepository.UpdateUserAsync(userDTO);
        }

        public async Task<SpResult> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }
        #endregion
    }
}
