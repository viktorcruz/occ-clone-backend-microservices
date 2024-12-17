using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO
{
    public class LoginUserDTO
    {
        [Required]
        public string Email { get; set; }

        [PasswordPropertyText]
        [Required, MinLength(8, ErrorMessage = "The password must be minimum 8 characters"), MaxLength(20, ErrorMessage = "The password must be maximum 20 characters")]
        public string Password { get; set; }

        public LoginUserDTO(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
