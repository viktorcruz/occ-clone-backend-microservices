using AuthService.Application.DTO;

namespace AuthService.Application.Commands
{
    public class ConfirmRegisterCommand
    {
        public string Email { get; }

        public ConfirmRegisterCommand(ConfirmRegistrationDTO confirmDTO)
        {
            Email = confirmDTO.Email;
        }
    }
}
