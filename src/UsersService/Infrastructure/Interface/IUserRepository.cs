using SharedKernel.Common.Responses;
using UsersService.Application.Dto;

namespace UsersService.Infrastructure.Interface
{
    public interface IUserRepository
    {
        Task<DatabaseResult> CreateUserAsync(AddUserDTO userDTO);
        Task<RetrieveDatabaseResult<UserRetrieveDTO>> GetUserByIdAsync(int userId);
        Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> GetAllUsersAsync();
        Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> SearchUsersAsync();
        Task<DatabaseResult> UpdateUserAsync(UserRetrieveDTO userDTO);
        Task<DatabaseResult> UpdateUserProfileAsync(UserProfileDTO userDTO);
        Task<DatabaseResult> DeleteUserAsync(int userId);
    }
}
