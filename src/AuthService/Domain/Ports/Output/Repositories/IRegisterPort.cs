using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Domain.Ports.Output.Repositories
{
    public interface IRegisterPort
    {
        Task<RegisterUserDTO> RegisterAsync(RegisterCommand command);
    }
}
