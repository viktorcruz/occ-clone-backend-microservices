using AuthService.Application.DTO;

namespace AuthService.Application.Commands
{
    public class RenewCommand
    {
        #region Properties
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        #endregion

        #region Constructor
        public RenewCommand(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
        #endregion
    }
}
