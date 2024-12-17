namespace UsersService.Application.DTO
{
    public class SearchUsersDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsRegistrationConfirmed { get; set; }
    }
}
