namespace UsersService.Application.Dto
{
    public class UserRetrieveDTO
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime RegistrationConfirmedAt { get; set; }
        public int IsRegistrationConfirmed { get; set; }
        public int IsActive { get; set; }
    }
}
