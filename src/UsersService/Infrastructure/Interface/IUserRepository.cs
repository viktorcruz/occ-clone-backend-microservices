using SharedKernel.Common.Responses;
using UsersService.Application.Dto;

namespace UsersService.Infrastructure.Interface
{
    public interface IUserRepository
    {
        Task<RetrieveDatabaseResult<UserRetrieveDTO>> GetUserByIdAsync(int userId);
        Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> GetAllUsersAsync();
        Task<RetrieveDatabaseResult<List<SearchUsersDTO>>> SearchUsersAsync(string firstName, string lastName, string email);
        Task<DatabaseResult> UpdateUserAsync(UserRetrieveDTO userDTO);
        Task<DatabaseResult> UpdateUserProfileAsync(UserProfileDTO userDTO);
        Task<DatabaseResult> DeleteUserAsync(int userId);
    }
}
