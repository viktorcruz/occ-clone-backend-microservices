using UsersService.Application.Dto;
using UsersService.SharedKernel.Common.Response;

namespace UsersService.Infrastructure.Interface
{
    public interface IUserRepository
    {
        Task<SpResult> CreateUserAsync(AddUserDTO userDTO);
        Task<SpRetrieveResult<UserRetrieveDTO>> GetUserByIdAsync(int userId);
        Task<SpRetrieveResult<List<UserRetrieveDTO>>> GetAllUsersAsync();
        Task<SpResult> UpdateUserAsync(UserRetrieveDTO userDTO);
        Task<SpResult> DeleteUserAsync(int userId);
    }
}
