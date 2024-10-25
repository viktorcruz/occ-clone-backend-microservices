namespace AuthService.Application.Commands
{
    public class RegisterCommand
    {
        public int IdRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public RegisterCommand(int idRole, string firstName, string lastName, string email, string password)
        {
            this.IdRole = idRole;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }
    }

}
