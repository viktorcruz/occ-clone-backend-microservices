using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Ports.Output;
using AuthService.Domain.Ports.Output.Services;

namespace AuthService.Application.UsesCases
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly ILoginPort _loginPort;
        private readonly ITokenService _tokenService;

        public LoginUseCase(ILoginPort loginPort, ITokenService tokenService)
        {
            _loginPort = loginPort;
            _tokenService = tokenService;
        }

        public async Task<LoginUserResponseDTO> Execute(LoginCommand command)
        {
            var response = await _loginPort.LoginAsync(command);
            if (response == null || string.IsNullOrEmpty(response.Token))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return response;
        }
    }
}
