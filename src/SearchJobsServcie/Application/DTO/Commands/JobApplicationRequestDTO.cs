using System.ComponentModel.DataAnnotations;

namespace SearchJobsService.Application.DTO.Commands
{
    public class JobApplicationRequestDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdPublication { get; set; }
      
        [Required]
        [Range(1, int.MaxValue)]
        public int IdApplicant { get; set; }
        
        [Required(ErrorMessage = "ApplicantName is required")]
        [StringLength(50)]
        public string ApplicantName { get; set; }
        
        [StringLength(50)]
        public string? ApplicantResume { get; set; }
        
        [StringLength(50)]
        public string? CoverLetter { get; set; }

        public int Status { get; set; }
    }
}
