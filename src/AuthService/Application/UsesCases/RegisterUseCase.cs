using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Ports.Output;
using SharedKernel.Interface;

namespace AuthService.Application.UsesCases
{
    public class RegisterUseCase : IRegisterUseCase
    {
        private readonly IRegisterPort _registerPort;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;

        public RegisterUseCase(IRegisterPort registerPort, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _registerPort = registerPort;
            _applicationExceptionHandler = applicationExceptionHandler;
        }

        public async Task<RegisterUserDTO> Execute(RegisterCommand command)
        {
            try
            {
                var response = await _registerPort.RegisterAsync(command);

                if (response == null || string.IsNullOrEmpty(response.Email))
                {
                    throw new("The email has already been registered");
                }

                return response;
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Application, ActionType.Register);
                throw;
            }
        }
    }
}
