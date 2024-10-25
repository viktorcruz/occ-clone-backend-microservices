using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Domain.Ports.Output.Repositories
{
    public interface ILoginPort
    {
        Task<LoginUserResponseDTO> LoginAsync(LoginCommand command);
    }
}
