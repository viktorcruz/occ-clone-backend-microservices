using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO
{
    public class RegisterUserDTO
    {
        [Required, Range(1, int.MaxValue)]
        public int IdRole { get; set; }

        [Required, MinLength(4)]
        public string FirstName { get; set; }

        [Required, MinLength(4)]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required, MinLength(8, ErrorMessage ="The password must be minimum 8 characters"), MaxLength(20, ErrorMessage = "The password must be maximum 20 charecters")]
        public string Password { get; set; }
    }
}
