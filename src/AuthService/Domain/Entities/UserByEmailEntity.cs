namespace AuthService.Domain.Entities
{
    public class UserByEmailEntity
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
