using SharedKernel.DTO;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO
{
    public class ConfirmRegisterResponseDTO 
    {
        [Required, Range(1, int.MaxValue)]
        public int IdUser { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public string ActiveStatusDescription { get; set; }

        [Required]
        public bool IsRegistrationConfirmed { get; set; }

        [Required]
        public string RegistrationConfirmationDescription { get; set; } 
    }
}
