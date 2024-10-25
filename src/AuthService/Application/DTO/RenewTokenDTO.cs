namespace AuthService.Application.DTO
{
    public class RenewTokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
