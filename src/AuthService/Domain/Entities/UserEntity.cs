namespace AuthService.Domain.Entities
{
    public class UserEntity
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
