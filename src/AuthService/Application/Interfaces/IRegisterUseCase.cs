using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Application.Interfaces
{
    public interface IRegisterUseCase
    {
        Task<RegisterUserDTO> Execute(RegisterCommand command);
    }
}
