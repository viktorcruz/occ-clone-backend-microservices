using SearchJobsService.Domain.Enum;

namespace SearchJobsService.Application.Dto
{
    public class JobApplicationRequestDTO
    {
        public int IdPublication { get; set; }
        public int IdApplicant { get; set; }
        public string ApplicantName { get; set; }
        public string? ApplicantResume { get; set; }
        public string? CoverLetter { get; set; }
    }
}
