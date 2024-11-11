namespace AuthService.Application.Commands
{
    public class ConfirmRegisterCommand
    {
        public string Email { get; }

        public ConfirmRegisterCommand(string email)
        {
            Email = email;
        }
    }
}
