namespace SearchJobsServcie.Application.Dto
{
    public class ApplyForJobDTO
    {
        public Guid IdJob { get; set; }
        public string IdUser { get; set; }
        public string CoverLetter { get; set; }
        public DateTime ApplicationDate { get;set; }
    }
}
