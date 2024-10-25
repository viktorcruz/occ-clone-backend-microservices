namespace AuthService.Application.DTO
{
    public class LoginUserResponseDTO
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
