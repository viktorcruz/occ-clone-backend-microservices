
using Microsoft.Identity.Client;

namespace SearchJobsService.Application.Dto
{
    public class UserApplicationsResponseDTO
    {
        public int IdPublication { get; set; }
        public int IdApplicant { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantResume { get; set; }
        public string CoverLetter { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }

    }
}
