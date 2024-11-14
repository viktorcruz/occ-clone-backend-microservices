namespace UsersService.Domain.Entity
{
    public class UserByEmailEntity
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int IsActive { get; set; }
        public int IsRegistrationConfirmed { get; set; }
    }
}
