using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Application.Interfaces
{
    public interface ILoginUseCase
    {
        Task<LoginUserResponseDTO> Execute(LoginCommand command);
    }
}
