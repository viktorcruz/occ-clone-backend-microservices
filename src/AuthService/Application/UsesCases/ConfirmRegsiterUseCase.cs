using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Ports.Output;

namespace AuthService.Application.UsesCases
{
    public class ConfirmRegsiterUseCase : IConfirRegisterUserCase
    {
        private readonly IConfirmRegisterPort _confirmRegisterPort;

        public ConfirmRegsiterUseCase(IConfirmRegisterPort confirmRegisterPort)
        {
            _confirmRegisterPort = confirmRegisterPort;
        }

        public async Task<ConfirmRegisterResponseDTO> Execute(ConfirmRegisterCommand command)
        {
            var response = await _confirmRegisterPort.ConfirmRegisterAsync(command);
            if (response == null)
            {
                throw new Exception("Error");
            }

            return response;
        }
    }
}
