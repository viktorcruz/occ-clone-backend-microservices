using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO
{
    public class LoginUserResponseDTO
    {
        [Required, Range(1, int.MaxValue)]
        public int IdUser { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int IdRole { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Email { get; set; }

        public string Token { get; set; }
    }
}
