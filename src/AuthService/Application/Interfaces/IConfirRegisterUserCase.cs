using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Application.Interfaces
{
    public interface IConfirRegisterUserCase
    {
        Task<ConfirmRegisterResponseDTO> Execute(ConfirmRegisterCommand command);
    }
}
