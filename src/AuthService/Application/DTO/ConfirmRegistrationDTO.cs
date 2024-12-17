using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO
{
    public class ConfirmRegistrationDTO
    {
        [Required]
        public string Email { get; set; }
    }
}
