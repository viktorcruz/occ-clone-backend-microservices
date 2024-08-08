namespace UsersService.Domain.Entity
{
    public class UserEntity
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string? RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
