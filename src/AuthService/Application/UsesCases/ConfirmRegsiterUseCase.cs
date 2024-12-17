using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Ports.Output;
using SharedKernel.Interfaces.Exceptions;

namespace AuthService.Application.UsesCases
{
    public class ConfirmRegsiterUseCase : IConfirRegisterUserCase
    {
        private readonly IConfirmRegisterPort _confirmRegisterPort;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;

        public ConfirmRegsiterUseCase(IConfirmRegisterPort confirmRegisterPort, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _confirmRegisterPort = confirmRegisterPort;
            _applicationExceptionHandler = applicationExceptionHandler;
        }

        public async Task<ConfirmRegisterResponseDTO> Execute(ConfirmRegisterCommand command)
        {
            try
            {
                var response = await _confirmRegisterPort.ConfirmRegisterAsync(command);
                if (response == null)
                {
                    throw new Exception("Faleid registration confirmation");
                }

                return response;
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Application, ActionType.Update);
                throw;
            }
        }
    }
}
