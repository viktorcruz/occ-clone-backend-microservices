using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Domain.Ports.Output
{
    public interface IConfirmRegisterPort
    {
        Task<ConfirmRegisterResponseDTO> ConfirmRegisterAsync(ConfirmRegisterCommand command);
    }
}
