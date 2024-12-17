namespace SharedKernel.Events.User
{
    // Evento emitido por UserService al crear un usuario
    public class UserCreatedEvent
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
