using SharedKernel.Common.Responses;
using UsersService.Application.Dto;
using UsersService.Domain.Entity;

namespace UsersService.Infrastructure.Interface
{
    public interface IUserRepository
    {
        Task<RetrieveDatabaseResult<UserRetrieveDTO>> GetUserByIdAsync(int userId);
        Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> GetAllUsersAsync();
        Task<RetrieveDatabaseResult<UserByEmailEntity>> GetByEmailAsync(string email);
        Task<RetrieveDatabaseResult<List<SearchUsersDTO>>> SearchUsersAsync(string firstName, string lastName, string email);
        Task<DatabaseResult> UpdateUserAsync(UserRetrieveDTO userDTO);
        Task<DatabaseResult> UpdateUserProfileAsync(UserProfileDTO userDTO);
        Task<DatabaseResult> ChangeUserPasswordAsync(int userId, string password, string email);
        Task<DatabaseResult> DeleteUserAsync(int userId);
    }
}
