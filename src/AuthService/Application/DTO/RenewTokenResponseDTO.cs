using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO
{
    public class RenewTokenResponseDTO
    {
        [Required, Range(1, int.MaxValue)]
        public int IdUser { get; set; }

        [Required]
        public string? Email { get; set; }
    }
}
