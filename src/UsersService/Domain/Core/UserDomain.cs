using SharedKernel.Common.Responses;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Interface;

namespace UsersService.Domain.Core
{
    public class UserDomain : IUserDomain
    {
        #region Properties
        public readonly IUserRepository _userRepository;
        #endregion

        #region Methods
        public UserDomain(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<DatabaseResult> CreateUserAsync(AddUserDTO userDTO)
        {
            return await _userRepository.CreateUserAsync(userDTO);
        }

        public async Task<RetrieveDatabaseResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> SearchUserAsync()
        {
            return await _userRepository.SearchUserAsync();
        }

        public async Task<DatabaseResult> UpdateUserAsync(UserRetrieveDTO userDTO)
        {
            return await _userRepository.UpdateUserAsync(userDTO);
        }

        public async Task<DatabaseResult> UpdateUserProfileAsync(UserProfileDTO userDTO)
        {
            return await _userRepository.UpdateUserProfileAsync(userDTO);
        }

        public async Task<DatabaseResult> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }


        #endregion
    }
}
