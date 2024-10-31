namespace UsersService.Application.Dto
{
    public class UserPasswordDTO
    {
        public int IdUser { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
