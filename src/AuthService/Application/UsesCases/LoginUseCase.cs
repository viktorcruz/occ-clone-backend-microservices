using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Ports.Output;
using SharedKernel.Interfaces.Exceptions;

namespace AuthService.Application.UsesCases
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly ILoginPort _loginPort;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;

        public LoginUseCase(ILoginPort loginPort, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _loginPort = loginPort;
            _applicationExceptionHandler = applicationExceptionHandler;
        }

        public async Task<LoginUserResponseDTO> Execute(LoginCommand command)
        {
            try
            {
                var response = await _loginPort.LoginAsync(command);
                if (response == null || string.IsNullOrEmpty(response.Token))
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                return response;
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Application, ActionType.Execute);
                throw;
            }
        }
    }
}
