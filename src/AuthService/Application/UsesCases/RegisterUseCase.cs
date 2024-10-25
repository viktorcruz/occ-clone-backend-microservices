using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Exceptions;
using AuthService.Domain.Ports.Output.Repositories;

namespace AuthService.Application.UsesCases
{
    public class RegisterUseCase : IRegisterUseCase
    {
        private readonly IRegisterPort _registerPort;

        public RegisterUseCase(IRegisterPort registerPort)
        {
            _registerPort = registerPort;
        }

        public async Task<RegisterUserDTO> Execute(RegisterCommand command)
        {
            var response = await _registerPort.RegisterAsync(command);

            if (response == null || string.IsNullOrEmpty(response.Email))
            {
                throw new UserAlreadyExistsException("The email has already been registered");
            }

            return response;
        }
    }
}
