using AuthService.Application.DTO;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Commands
{
    public class RenewCommand
    {
        #region Properties
        public string AccessToken { get; set; }

        [Required]
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
