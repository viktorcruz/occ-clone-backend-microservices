namespace SearchJobsServcie.Application.Dto
{
    public class JobApplicationDTO
    {
        public Guid IdJob { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public DateTime ApplicationDate { get; set; }
    }
}
